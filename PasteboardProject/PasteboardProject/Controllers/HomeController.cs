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
    
    [HttpGet("myaccount")]
    public IActionResult MyAccount()
    {
        
        if (HttpContext.User.Identity.IsAuthenticated)
        {
            return RedirectToAction("GetUserPage", "User", HttpContext.User.Identity);
        }

        return View("~/Views/Error/ErrorPage.cshtml", "Выполните вход или зарегистрируйтесь");
    }
}