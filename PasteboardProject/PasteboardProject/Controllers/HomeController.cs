using Microsoft.AspNetCore.Mvc;

namespace PasteboardProject.Controllers;

[Route("")]
public class HomeController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("instruction")]
    public IActionResult Instruction()
    {
        return View();
    }
    
    [HttpGet("policy")]
    public IActionResult Policy()
    {
        return View();
    }
}