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
        var activeFields = pasteboardById.PasteboardFields.Where(pf => pf.FieldName != null && pf.FieldValue != null);
        pasteboardById.PasteboardFields = activeFields.ToList();
        return View(pasteboardById);
    }
    
    [HttpGet]
    public IActionResult CreatePasteboard()
    {
        ViewBag.Header = "Create";
        var pasteboard = new Pasteboard
        {
            PasteboardFields = new List<PasteboardField>
            {
                new PasteboardField(),
                new PasteboardField(),
                new PasteboardField(),
                new PasteboardField(),
                new PasteboardField(),
                new PasteboardField(),
                new PasteboardField(),
                new PasteboardField(),
                new PasteboardField(),
                new PasteboardField()
            }
        };
        return View("CreateEditPasteboard",pasteboard);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePasteboard(Pasteboard pasteboard)
    {
        await pasteboardRepository.AddCardToJsonAsync(pasteboard);
        var id = pasteboard.Id;
        return RedirectToAction("ShowPasteboard", new {id});
    }

    [HttpGet]
    public IActionResult EditPasteboard(int id)
    {
        ViewBag.Header = "Edit";
        var pasteboardById = pasteboardRepository.GetPasteboardById(id);
        return View("CreateEditPasteboard", pasteboardById);
    }
}