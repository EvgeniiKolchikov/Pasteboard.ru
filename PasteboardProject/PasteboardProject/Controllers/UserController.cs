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

namespace PasteboardProject.Controllers;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private static readonly Logger Logger = LogManager.GetLogger("UserController");
    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        Logger.Debug("User Controller in");
    }

    [HttpGet("create")]
    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePasteboard(User user)
    {
        
    }
}