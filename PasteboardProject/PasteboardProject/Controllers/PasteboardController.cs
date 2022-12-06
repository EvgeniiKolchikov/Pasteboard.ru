using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NLog.Fluent;
using PasteboardProject.Models;
using PasteboardProject.Repositories;

namespace PasteboardProject.Controllers;

public class PasteboardController : Controller
{
    private PasteboardRepository pasteboardRepository;
    public PasteboardController()
    {
        pasteboardRepository = new PasteboardRepository();
    }
    [HttpGet]
    public IActionResult ShowPasteboard(int id)
    {
        var pasteboardById = pasteboardRepository.GetPasteboardById(id); 
        return View(pasteboardById);
    }
    
    [HttpGet]
    public IActionResult CreatePasteboard()
    {
        var pasteboard = new Pasteboard()
        {
            PasteboardFields = new List<PasteboardField>()
        };
        pasteboardRepository.CreatePasteboard(pasteboard);
        var count = pasteboardRepository.Pasteboards.Count;
        return View();
    }
    [HttpPost]
    public IActionResult CreatePasteboard(Pasteboard pasteboard)
    {
        pasteboardRepository.CreatePasteboard(pasteboard);
        var count = pasteboardRepository.Pasteboards.Count;
        Log.Debug("CardController created");
        return View();
    }
}