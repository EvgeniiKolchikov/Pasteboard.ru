using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        var activeFields = pasteboardById.PasteboardFields.Where(pf => pf.FieldName != null && pf.FieldValue != null);
        pasteboardById.PasteboardFields = activeFields.ToList();
        return View(pasteboardById);
    }
    
    [HttpGet]
    public IActionResult CreatePasteboard()
    {
        var pasteboardFields = new List<PasteboardField>();
        for (int i = 0; i < 10; i++)
        {
            pasteboardFields.Add(new PasteboardField());
        }
        var pasteboard = new Pasteboard
        {
            PasteboardFields = pasteboardFields
        };
        return View("CreateEditPasteboard",pasteboard);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePasteboard(Pasteboard pasteboard)
    {
        if (ModelState.IsValid)
        {
            await pasteboardRepository.AddCardToJsonAsync(pasteboard);
            var id = pasteboard.Id;
            return RedirectToAction("ShowPasteboard", new {id});
        }
        else
        {
            return View("CreateEditPasteboard", pasteboard);
        }
    }

    [HttpGet]
    public IActionResult EditPasteboard(int id)
    {
        var pasteboardById = pasteboardRepository.GetPasteboardById(id);
        return View("CreateEditPasteboard", pasteboardById);
    }

    [HttpPost]
    public async Task<IActionResult> EditPasteboard(Pasteboard pasteboard)
    {
        await pasteboardRepository.AddCardToJsonAsync(pasteboard);
        var id = pasteboard.Id;
        return RedirectToAction("ShowPasteboard", new{id});
    }
}