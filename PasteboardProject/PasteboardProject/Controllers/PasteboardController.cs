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
    public IActionResult ShowPasteboard(string id)
    {
        
        var pasteboardById = _repository.GetPasteboardById(id);
        return View(pasteboardById.Result);
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
    
    [HttpGet]
    public IActionResult EditPasteboard(string id)
    {
        var pasteboardById = _repository.GetPasteboardById(id).Result;
        var listActivePasteboardFields = new List<ActivePasteboardField>();
        foreach (var pasteboardField in pasteboardById.PasteboardFields)
        {
            listActivePasteboardFields.Add(new ActivePasteboardField{FieldName = pasteboardField.FieldName,FieldValue = pasteboardField.FieldValue});
        }

        listActivePasteboardFields = AddEmptyFields(listActivePasteboardFields);
        var pasteboardViewModel = new PasteboardViewModel
        {
            Pasteboard = pasteboardById,
            AspAction = "EditPasteboard",
            ActivePasteboardFields = listActivePasteboardFields
        };
        return View("CreateEditPasteboard", pasteboardViewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> EditPasteboard(PasteboardViewModel pasteboardViewModel)
    {
        var pasteboard = new Pasteboard();
            pasteboard = DeleteEmptyFields(pasteboardViewModel);
        await _repository.AddPasteboardAsync(pasteboard);
        var id = pasteboard.Id;
        return RedirectToAction("ShowPasteboard", new{id});
    }
    
    
    private List<ActivePasteboardField> AddEmptyFields(List<ActivePasteboardField> activePasteboardField)
    {
        var maxFieldCount = 10;
        var activeFieldCount = activePasteboardField.Count(pf => pf.FieldName != null && pf.FieldValue != null);
       
        for (int i = activeFieldCount; i < maxFieldCount; i++)
        {
            activePasteboardField.Add(new ActivePasteboardField());
        }
        if (activeFieldCount == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                activePasteboardField[i].IsActive = true;
            }
        }
        else
        {
            for (int i = 0; i < activeFieldCount; i++)
            {
                activePasteboardField[i].IsActive = true;
            }
        }
        return activePasteboardField;
    }
    private Pasteboard DeleteEmptyFields(PasteboardViewModel pasteboardViewModel)
    {
        // foreach (var pf in pasteboardViewModel.ActivePasteboardFields)
        // {
        //     if (pf.FieldName is not null && pf.FieldValue is not null)
        //     {
        //         pf.IsActive = true;
        //     }
        // }
        var listWithActiveFields = pasteboardViewModel.ActivePasteboardFields.Where(pf => pf.IsActive).ToList();
        var pasteboardFields = new List<PasteboardField>();
        foreach (var activePasteboardField in listWithActiveFields)
        {
            pasteboardFields.Add(new PasteboardField{FieldName = activePasteboardField.FieldName, FieldValue = activePasteboardField.FieldValue});
        }

        pasteboardViewModel.Pasteboard.PasteboardFields = pasteboardFields;
        
        return pasteboardViewModel.Pasteboard;
    }
}