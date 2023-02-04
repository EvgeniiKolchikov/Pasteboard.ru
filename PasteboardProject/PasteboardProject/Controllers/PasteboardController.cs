using Microsoft.AspNetCore.Mvc;
using PasteboardProject.Exceptions;
using PasteboardProject.Interfaces;
using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;
using PasteboardProject.Repositories;

namespace PasteboardProject.Controllers;

[Route("[controller]")]
public class PasteboardController : Controller
{
    private IRepository _repository;
    public PasteboardController(IRepository repository)
    {
        _repository = repository;
    }
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> ShowPasteboard(string id)
    {
        try
        {
            var pasteboardById = await _repository.GetPasteboardByIdAsync(id);
            return View(pasteboardById);
        }
        catch (CustomException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception)
        {
            return NotFound(CustomException.DefaultMessage); // to ErrorView
        }
    }
    
    [HttpGet]
    [Route("create")]
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
    [Route("create")]
    public async Task<IActionResult> CreatePasteboard(PasteboardViewModel pasteboardViewModel)
    {
        var pasteboard = DeleteEmptyFields(pasteboardViewModel);
        await _repository.AddPasteboardAsync(pasteboard);
        var id = pasteboardViewModel.Pasteboard.Id;
        return RedirectToAction("ShowPasteboard", new {id});
    }
    
    [HttpGet]
    [Route("edit/{id}")]
    public IActionResult EditPasteboard(string id)
    {
        try
        {
            var pasteboardById = _repository.GetPasteboardByIdAsync(id).Result;
            var listActivePasteboardFields = new List<ActivePasteboardField>();
            foreach (var pasteboardField in pasteboardById.PasteboardFields)
            {
                listActivePasteboardFields.Add(new ActivePasteboardField
                    { FieldName = pasteboardField.FieldName, 
                      FieldValue = pasteboardField.FieldValue 
                    });
            }
            var pasteboardViewModel = new PasteboardViewModel
            {
                Pasteboard = pasteboardById,
                AspAction = "EditPasteboard",
                ActivePasteboardFields = AddEmptyFields(listActivePasteboardFields)
            };
            return View("CreateEditPasteboard", pasteboardViewModel);
        }
        catch (CustomException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception)
        {
            return NotFound(CustomException.DefaultMessage);
        }
    }
    
    [HttpPost]
    [Route("edit/{id}")]
    public async Task<IActionResult> EditPasteboard(PasteboardViewModel pasteboardViewModel)
    {
        var pasteboard = DeleteEmptyFields(pasteboardViewModel);
        await _repository.AddPasteboardAsync(pasteboard);
        var id = pasteboard.Id;
        return RedirectToAction("ShowPasteboard", new{id});
    }
    private List<ActivePasteboardField> AddEmptyFields(List<ActivePasteboardField> activePasteboardField)
    {
        var maxFieldCount = 10;
        var activeFieldCount = activePasteboardField
            .Count(pf => pf.FieldName != null && pf.FieldValue != null);
        for (var i = activeFieldCount; i < maxFieldCount; i++)
        {
            activePasteboardField.Add(new ActivePasteboardField());
        }
        if (activeFieldCount == 0)
        {
            for (var i = 0; i < 3; i++)
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
        var pasteboardFields = new List<PasteboardField>();
        foreach (var activePasteboardField in pasteboardViewModel.ActivePasteboardFields.Where(pf => pf.IsActive))
        {
            pasteboardFields.Add(new PasteboardField
            {
                FieldName = activePasteboardField?.FieldName??"", 
                FieldValue = activePasteboardField?.FieldValue??""
            });
        }
        pasteboardViewModel.Pasteboard.PasteboardFields = pasteboardFields;
        return pasteboardViewModel.Pasteboard;
    }
}