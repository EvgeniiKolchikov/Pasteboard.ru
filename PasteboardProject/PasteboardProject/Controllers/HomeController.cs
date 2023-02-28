using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PasteboardProject.Controllers;

[Route("")]
public class HomeController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        ViewBag.Name = User.FindFirstValue(ClaimTypes.Email);
        ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
        return View();
    }

    [HttpGet("instruction")]
    public IActionResult Instruction()
    {
        return View();
    }
}