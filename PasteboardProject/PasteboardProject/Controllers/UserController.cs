using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NLog;
using PasteboardProject.Exceptions;
using PasteboardProject.Interfaces;
using PasteboardProject.Models.ViewModels;
using PasteboardProject.Services;

namespace PasteboardProject.Controllers;


[Route("user")]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private static readonly Logger Logger = LogManager.GetLogger("UserController");
    public UserController(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _emailService = emailService;

        Logger.Debug("User Controller in");
    }
    
    [HttpGet("register")]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        try
        {
            if (!ModelState.IsValid) return View(registerViewModel);
            var userExist = await _userRepository.ExistUserInDataBaseAsync(registerViewModel);
            if (userExist)
            {
                ModelState.AddModelError("Email","Эл. почта уже существует, введите другой или войдите");
                return View(registerViewModel);
            }
            await _userRepository.AddUserToDataBaseAsync(registerViewModel);
            var emailToken = await _userRepository.GetUserToken(registerViewModel.Email);
            await _emailService.SendEmailAsync(registerViewModel.Email,emailToken);
            var id = "register";
            return RedirectToAction("VerifyUser", new {id});
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e.Data, e.StackTrace);
            return View("~/Views/Error/ErrorPage.cshtml", CustomException.DefaultMessage);
        }
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View("Login");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid) return View(loginViewModel);
        try
        {
            var userEmailConfirmed = await _userRepository.EmailConfirmedCheck(loginViewModel.Email);
            if (!userEmailConfirmed)
            {
                ModelState.AddModelError("Email","Выполните подтверждение аккаунта через эл.почту");
                return View(loginViewModel);
            }
            TokenService.AddTokenToCookie(loginViewModel,_configuration,HttpContext);
            return RedirectToAction("UserPage");
        }
        catch (CustomException)
        {
            ModelState.AddModelError("Email", "Неверный пароль или email");
            ModelState.AddModelError("Password", "Неверный пароль или email");
            return View(loginViewModel);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e.Data, e.StackTrace);
            return View("~/Views/Error/ErrorPage.cshtml", CustomException.DefaultMessage);
        }
    }
    
    [Authorize]
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        TokenService.DeleteTokenFromCookie(HttpContext);
        return RedirectToAction("Index","Home");
    }
    
    [HttpGet("verify/{id}")]
    public async Task<IActionResult> VerifyUser(string id)
    {
        if (id == "register")
        {
            return View("VerifyUser", "Подтвердите аккаунт через эл. почту");
        }
        var tokenConfirmed = await _userRepository.UserTokenConfirmation(id);
        if (!tokenConfirmed) return View("~/Views/Error/ErrorPage.cshtml", CustomException.AccessDeniedMessage);
        return View("VerifyUser","Подтверждение эл.почты прошло успешно, выполните вход для дальнейшей работы");
    }
    
    [Authorize]
    [HttpGet("edit")]
    public IActionResult EditUser()
    {
        var editViewModel = new EditViewModel
        {
            Email = User.FindFirstValue(ClaimTypes.Email)
        };
        return View(editViewModel);
    }
    
    [Authorize]
    [HttpPost("edit")]
    public async Task<IActionResult> EditUser(EditViewModel editViewModel)
    {
        if (!ModelState.IsValid) return View(editViewModel);
        await _userRepository.UpdateUserAsync(editViewModel);
        return RedirectToAction("UserPage");
    }

    [Authorize(Roles = "User, Administrator")]
    [HttpGet("profile")]
    public async Task<IActionResult> UserPage()
    {
        try
        {
            var userViewModel = await _userRepository.GetUserViewModelAuthorizedAsync
                (User.FindFirstValue(ClaimTypes.Email));
            return View(userViewModel);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e.Data, e.StackTrace);
            return View("~/Views/Error/ErrorPage.cshtml", CustomException.DefaultMessage);
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("admin")]
    public async Task<IActionResult> AdminPage()
    {
        var userList = await _userRepository.GetUserListAsync();
        return View("~/Views/Admin/AdminPage.cshtml",userList);
    }
}