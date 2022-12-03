using Microsoft.AspNetCore.Mvc;
using PasteboardProject.Models;
using PasteboardProject.Repositories;

namespace PasteboardProject.Controllers;

public class PasteboardController : Controller
{
    private PasteboardRepository repo;
    public IActionResult Index(int Id)
    {
        repo = new PasteboardRepository();
        var pasteboardById = repo.GetPasteboardById(Id);
        var pasteboardFieldsById = repo.GetFieldsByPasteboardId(Id);
        var pasteboardViewModel = new PasteboardViewModel
            { Pasteboard = pasteboardById, PasteboardFields = pasteboardFieldsById };
        
        return View(pasteboardViewModel);
    }
}