using Microsoft.AspNetCore.Mvc;
using PasteboardProject.Interfaces;
using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;
using PasteboardProject.Repositories;

namespace PasteboardProject.Controllers;

public class PasteboardController : Controller
{
    private IRepository _repository;
    public PasteboardController(IRepository repository)
    {
        _repository = repository;
    }
    [HttpGet]
    public IActionResult ShowPasteboard(int id)
    {
        var pasteboardById = _repository.GetPasteboardById(id);
        return View(pasteboardById);
    }
    
    [HttpGet]
    public IActionResult CreatePasteboard()
    {
        var pasteboard = new Pasteboard();
        var activePasteboardFields = new List<ActivePasteboardField>();
        var pasteboardViewModel = new PasteboardViewModel()
        {
            Pasteboard = pasteboard,
            AspAction = "CreatePasteboard",
            ActivePasteboardFields = AddEmptyFields(activePasteboardFields)
        };
        return View("CreateEditPasteboard",pasteboardViewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePasteboard(PasteboardViewModel pasteboardViewModel)
    {
        var pasteboard = new Pasteboard();
        pasteboard = DeleteEmptyFields(pasteboardViewModel);
        await _repository.AddPasteboardAsync(pasteboard);
        var id = pasteboardViewModel.Pasteboard.Id;
        return RedirectToAction("ShowPasteboard", new {id});
    }
    
    // [HttpGet]
    // public IActionResult EditPasteboard(int id)
    // {
    //     var pasteboardById = _repository.GetPasteboardById(id);
    //     var pasteboardViewModel = new PasteboardViewModel
    //     {
    //         Pasteboard = AddEmptyFields(pasteboardById),
    //         AspAction = "EditPasteboard"
    //     };
    //     return View("CreateEditPasteboard", pasteboardViewModel);
    // }
    
    // [HttpPost]
    // public async Task<IActionResult> EditPasteboard(PasteboardViewModel pasteboardViewModel)
    // {
    //     var pasteboard = new Pasteboard();
    //         pasteboard = DeleteEmptyFields(pasteboardViewModel);
    //     await _repository.AddPasteboardAsync(pasteboard);
    //     var id = pasteboard.Id;
    //     return RedirectToAction("ShowPasteboard", new{id});
    // }
    
    private List<ActivePasteboardField> AddEmptyFields(List<ActivePasteboardField> pasteboardField)
    {
        var maxFieldCount = 10;
        var activeFieldCount = pasteboardField.Count(pf => pf.IsActive);
       
        for (int i = activeFieldCount; i < maxFieldCount; i++)
        {
            pasteboardField.Add(new ActivePasteboardField());
        }
        if (activeFieldCount == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                pasteboardField[i].IsActive = true;
            }
        }
        return pasteboardField;
    }
    private Pasteboard DeleteEmptyFields(PasteboardViewModel pasteboardViewModel)
    {
        foreach (var pf in pasteboardViewModel.ActivePasteboardFields)
        {
            if (pf.FieldName is not null && pf.FieldValue is not null)
            {
                pf.IsActive = true;
            }
        }

        var listWithActiveFields = pasteboardViewModel.ActivePasteboardFields.Where(pf => pf.IsActive)
            .Select(pf => (pf.FieldName, pf.FieldValue)).ToList();
        for (var i = 0; i < pasteboardViewModel.Pasteboard.PasteboardFields.Count; i++)
        {
            pasteboardViewModel.Pasteboard.PasteboardFields[i].FieldName = listWithActiveFields[i].FieldName;
            pasteboardViewModel.Pasteboard.PasteboardFields[i].FieldValue = listWithActiveFields[i].FieldValue;
        }
        return pasteboardViewModel.Pasteboard;
    }
}