using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NLog.Fluent;
using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;
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
        var pasteboard = new Pasteboard();
        pasteboard = AddEmptyFields(pasteboard);
        var pasteboardViewModel = new PasteboardViewModel()
        {
            Pasteboard = pasteboard,
            AspAction = "CreatePasteboard"
        };
        return View("CreateEditPasteboard",pasteboardViewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePasteboard(PasteboardViewModel pasteboardViewModel)
    {
        var pasteboardToRepository = new Pasteboard();
        pasteboardToRepository = DeleteEmptyFields(pasteboardViewModel);
        await pasteboardRepository.AddCardToJsonAsync(pasteboardToRepository);
        var id = pasteboardViewModel.Pasteboard.Id;
        return RedirectToAction("ShowPasteboard", new {id});
    }
    
    [HttpGet]
    public IActionResult EditPasteboard(int id)
    {
        var pasteboardById = pasteboardRepository.GetPasteboardById(id);
        var pasteboardViewModel = new PasteboardViewModel
        {
            Pasteboard = AddEmptyFields(pasteboardById),
            AspAction = "EditPasteboard"
        };
        return View("CreateEditPasteboard", pasteboardViewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> EditPasteboard(PasteboardViewModel pasteboardViewModel)
    {
        var pasteboard = new Pasteboard();
            pasteboard = DeleteEmptyFields(pasteboardViewModel);
        await pasteboardRepository.AddCardToJsonAsync(pasteboard);
        var id = pasteboard.Id;
        return RedirectToAction("ShowPasteboard", new{id});
    }
    
    private Pasteboard AddEmptyFields(Pasteboard pasteboard)
    {
        var maxFieldCount = 10;
        var activeFieldCount = pasteboard.PasteboardFields.Count(pf => pf.IsActive);
       
        for (int i = activeFieldCount; i < maxFieldCount; i++)
        {
            pasteboard.PasteboardFields.Add(new ActivePasteboardField());
        }
        if (activeFieldCount == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                pasteboard.PasteboardFields[i].IsActive = true;
            }
        }
        return pasteboard;
    }
    private Pasteboard DeleteEmptyFields(PasteboardViewModel pasteboardViewModel)
    {
        var listWithActiveFields = pasteboardViewModel.Pasteboard.PasteboardFields.Where(pf => pf.IsActive).ToList();
        pasteboardViewModel.Pasteboard.PasteboardFields = listWithActiveFields;
        return pasteboardViewModel.Pasteboard;
    }
}