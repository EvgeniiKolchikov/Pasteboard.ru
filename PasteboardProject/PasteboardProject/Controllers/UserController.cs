using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using NLog;
using PasteboardProject.Exceptions;
using PasteboardProject.Interfaces;
using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;

namespace PasteboardProject.Controllers;


[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private static readonly Logger Logger = LogManager.GetLogger("UserController");
    private const int ExpireTokenTime = 60;
    private const int ExpireCookieTime = 60;
   
    public UserController(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
       
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
        var userExist = await _userRepository.ExistUserInDataBaseAsync(registerViewModel);
        if (userExist) return View(registerViewModel); // добавить exception
        await _userRepository.AddUserToDataBaseAsync(registerViewModel);
        var token = GenerateToken(registerViewModel);
        AddTokenToCookie(token);
        var userViewModel = new UserViewModel()
            {
                Name = registerViewModel.Name, 
                Email = registerViewModel.Email, 
                Pasteboards = new List<Pasteboard>()
            };
        
        return RedirectToAction("UserPage", new {userViewModel});
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View("Login");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        try
        {
            var userViewModel = await _userRepository.GetUserViewModelLoginAsync(loginViewModel);
            var token = GenerateToken(loginViewModel);
            AddTokenToCookie(token);
            return RedirectToAction("UserPage",new {userViewModel});
        }
        catch (CustomException e)
        {
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
        DeleteTokenFromCookie();
        return RedirectToAction("Index","Home");
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
        await _userRepository.UpdateUserAsync(editViewModel);
        var userViewModel = await _userRepository.GetUserViewModelAuthorizedAsync(User.FindFirstValue(ClaimTypes.Email));
        return RedirectToAction("UserPage", new {userViewModel});
    }

    [Authorize(Roles = "User, Administrator")]
    [HttpGet("profile")]
    public async Task<IActionResult> UserPage()
    {
        if (!User.Identity.IsAuthenticated)
            return View("~/Views/Error/ErrorPage.cshtml", CustomException.AccessDeniedMessage);
        var userViewModel = await _userRepository.GetUserViewModelAuthorizedAsync
            (User.FindFirstValue(ClaimTypes.Email));
        return View(userViewModel);
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("admin")]
    public async Task<IActionResult> AdminPage()
    {
        var userList = await _userRepository.GetUserListAsync();
        return View("~/Views/Admin/AdminPage.cshtml",userList);
    }
    private string GenerateToken(ITokenGenerated user)
    {
        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Email, user.Email));
        if (user.Email == "admin@pasteboard.ru")
        {
            claims.Add(new Claim(ClaimTypes.Role,"Administrator"));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role,"User"));
        }
        
        var keybytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var jwt = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(ExpireTokenTime)),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(keybytes),
                    SecurityAlgorithms.HmacSha256));
            
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private void AddTokenToCookie(string token)
    {
        HttpContext.Response.Cookies.Append(".AspNetCore.PasteboardCookie", token, 
            new CookieOptions
            {
                MaxAge = TimeSpan.FromMinutes(ExpireCookieTime)
            });
    }

    private void DeleteTokenFromCookie()
    {
        if (HttpContext.Request.Cookies.ContainsKey(".AspNetCore.PasteboardCookie"))
        {
            HttpContext.Response.Cookies.Delete(".AspNetCore.PasteboardCookie");
        }
    }
}