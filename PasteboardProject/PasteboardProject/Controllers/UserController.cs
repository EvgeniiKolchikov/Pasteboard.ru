using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        await _userRepository.AddUserToDataBase(registerViewModel);
        var token = GenerateToken(registerViewModel);
        AddTokenToCookie(token);
        var userViewModel = new UserViewModel()
            {
                Name = registerViewModel.Name, 
                Email = registerViewModel.Email, 
                Pasteboards = new List<Pasteboard>()
            };
        return View("UserPage", userViewModel);
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        try
        {
            var userViewModel = await _userRepository.GetUserAsync(loginViewModel);
            var token = GenerateToken(loginViewModel);
            AddTokenToCookie(token);
            return View("UserPage", userViewModel);
        }
        catch (CustomException e)
        {
        }
        catch (Exception e)
        {
            Logger.Error(e.Message, e.Data, e.StackTrace);
            return View("~/Views/Error/ErrorPage.cshtml", CustomException.DefaultMessage);
        }
        return View();
    }

    [HttpPost]
    public IActionResult Logout()
    {
        DeleteTokenFromCookie();
        return View("~/Views/Home/Home.cshtml");
    }
    

    private string GenerateToken(ITokenGenerated user)
    {
        var claims = new List<Claim> {new Claim(ClaimTypes.Email, user.Email) };
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
        HttpContext.Response.Cookies.Append(".AspNetCore.Cookies", token, 
            new CookieOptions
            {
                MaxAge = TimeSpan.FromMinutes(ExpireCookieTime)
            });
    }

    private void DeleteTokenFromCookie()
    {
        if (HttpContext.Request.Cookies.ContainsKey(".AspNetCore.Cookies"))
        {
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
        }
    }
}