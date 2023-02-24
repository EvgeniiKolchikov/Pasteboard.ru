using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NLog;
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
    public UserController(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        Logger.Debug("User Controller in");
    }
    
    [HttpGet("create")]
    public IActionResult CreateUser()
    {
        return View();
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(User user)
    {
        if (_userRepository.ExistUserInDataBaseAsync(user).Result)
        {
            return BadRequest("Пользователь с таким именем уже существует");
        }
        await _userRepository.AddUserToDataBase(user);
        var token = GenerateToken(user);
        AddTokenToCookie(token);
        return RedirectToAction("GetUserPage", user.Name);
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(User user)
    {
        try
        {
            var isLoginPassed = await _userRepository.CheckUserNameAndPasswordAsync(user);
            if (!isLoginPassed)
            {
                HttpContext.Response.StatusCode = 400;
                return View();
            }
            var token = GenerateToken(user);
            AddTokenToCookie(token);
            return RedirectToAction("GetUserPage",user);
        }
        catch (Exception e)
        {
            return View();
        }
    }
    
    [Authorize]
    public IActionResult GetUserPage(User user)
    {
        var userDb = _userRepository.GetUserAsync(user.Name).Result;
        return View("UserPage", userDb);
    }

    private string GenerateToken(User user)
    {
        var claims = new List<Claim> {new Claim(ClaimTypes.Name, user.Name) };
        var keybytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var jwt = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(keybytes),
                    SecurityAlgorithms.HmacSha256));
            
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private void AddTokenToCookie(string token)
    {
        HttpContext.Response.Cookies.Append(".AspNetCore.Cookies", token, 
            new CookieOptions
            {
                MaxAge = TimeSpan.FromMinutes(60)
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