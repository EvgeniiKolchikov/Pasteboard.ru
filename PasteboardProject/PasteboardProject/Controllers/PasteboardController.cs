using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NLog.Fluent;
using PasteboardProject.Models;
using PasteboardProject.Repositories;

namespace PasteboardProject.Controllers;

public class PasteboardController : Controller
{
    private PasteboardRepositoryJson pasteboardRepository;
    public PasteboardController()
    {
        pasteboardRepository = new PasteboardRepositoryJson();
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
        return View();
    }

    [HttpPost]
    // public IActionResult CreatePasteboard(Pasteboard pasteboard)
    // {
    //     var lastId = pasteboardRepository.Pasteboards.Count;
    //     pasteboard.Id = lastId + 1;
    //     pasteboardRepository.CreatePasteboard(pasteboard);
    //     var count = pasteboardRepository.Pasteboards.Count;
    //     Log.Debug("CardController created");
    //     return RedirectToAction("ShowPasteboard", pasteboard);
    // }
    public async Task<IActionResult> CreatePasteboard(Pasteboard pasteboard)
    {
        var lastId = pasteboardRepository.Pasteboards.Count;
        pasteboard.Id = lastId + 1;
        pasteboardRepository.Pasteboards.Add(pasteboard);
        await pasteboardRepository.AddCardToJsonAsync();
        object id = pasteboard.Id;
        return View();
    }
}