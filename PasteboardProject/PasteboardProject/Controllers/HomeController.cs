using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PasteboardProject.Controllers;

[Route("")]
public class HomeController : Controller
{
    [HttpGet("")]
    public IActionResult Home()
    {
        return View();
    }

    [HttpGet("instruction")]
    public IActionResult Instruction()
    {
        return View();
    }
}