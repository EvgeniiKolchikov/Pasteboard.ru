using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using PasteboardProject.Exceptions;
using PasteboardProject.Interfaces;
using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;
using PasteboardProject.Repositories;

namespace PasteboardProject.Controllers;

[Authorize]
[Route("[controller]")]
public class PasteboardController : Controller
{
    private readonly IPasteboardRepository _pasteboardRepository;
    private static readonly Logger Logger = LogManager.GetLogger("PasteboardController");
    public PasteboardController(IPasteboardRepository pasteboardRepository)
    {
        _pasteboardRepository = pasteboardRepository;
        Logger.Debug("Logger Init");
    }
    
    [AllowAnonymous]
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> ShowPasteboard(string id)
    {
        try
        {
            var pasteboardById = await _pasteboardRepository.GetPasteboardByIdAsync(id);
            Logger.Debug($"This is Show Pasteboard Action: Id {id}, pasteboard name: {pasteboardById.Name}");
            return View(pasteboardById);
        }
        catch (CustomException e)
        {
            Logger.Error($"CustomException: {e.Message}");
            return View("~/Views/Error/ErrorPage.cshtml", e.Message);
        }
        catch (Exception e)
        {
            Logger.Error($"Exception: {e.Message}");
            return View("~/Views/Error/ErrorPage.cshtml", e.Message);
        }
    }
    
    [HttpGet]
    [Route("create")]
    public IActionResult CreatePasteboard()
    {
        Logger.Debug($"This is CreatePasteboard Action: Get");
        var activePasteboardFields = new List<ActivePasteboardField>();
        var pasteboardViewModel = new PasteboardViewModel()
        {
            Id = 0,
            AspAction = "CreatePasteboard",
            ActivePasteboardFields = AddEmptyFields(activePasteboardFields)
        };
        return View("CreateEditPasteboard",pasteboardViewModel);
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreatePasteboard(PasteboardViewModel pasteboardViewModel)
    {
        Logger.Debug($"This is CreatePasteboard Action: Post");
        var pasteboard = DeleteEmptyFields(pasteboardViewModel);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        
        await _pasteboardRepository.SendPasteboardToDataBaseAsync(pasteboard, userEmail);
        var id = pasteboard.Id;
        return View("ShowPasteboard", pasteboard);
    }
    
    [HttpGet]
    [Route("edit/{id}")]
    public async Task<IActionResult> EditPasteboard(string id)
    {
        Logger.Debug($"This is EditPasteboard Action: Get");
        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var pasteboardById = await _pasteboardRepository.GetPasteboardByIdWithUserCheckAsync(id,userEmail);
            var listActivePasteboardFields = pasteboardById.PasteboardFields.Select(pasteboardField => new ActivePasteboardField { FieldName = pasteboardField.FieldName, FieldValue = pasteboardField.FieldValue }).ToList();
            var pasteboardViewModel = new PasteboardViewModel
            {
                Name = pasteboardById.Name,
                AspAction = "EditPasteboard",
                ActivePasteboardFields = AddEmptyFields(listActivePasteboardFields)
            };
            return View("CreateEditPasteboard", pasteboardViewModel);
        }
        catch (CustomException e)
        {
            Logger.Error($"CustomException: {e.Message}");
            return View("~/Views/Error/ErrorPage.cshtml", e.Message);
        }
        catch (Exception e)
        {
            Logger.Error($"Exception: {e.Message} {e.Data} {e.StackTrace}");
            return View("~/Views/Error/ErrorPage.cshtml", CustomException.DefaultMessage);
        }
    }
    
    [HttpPost]
    [Route("edit/{id}")]
    public async Task<IActionResult> EditPasteboard(PasteboardViewModel pasteboardViewModel)
    {
        Logger.Debug($"This is EditPasteboard Action: Post");
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var pasteboard = DeleteEmptyFields(pasteboardViewModel);
        await _pasteboardRepository.SendPasteboardToDataBaseAsync(pasteboard, userEmail);
        var id = pasteboard.Id;
        return RedirectToAction("ShowPasteboard", new{id});
    }

    [HttpPost]
    public async Task DeletePasteboard(Pasteboard pasteboard)
    {
        await _pasteboardRepository.DeletePasteboard(pasteboard);
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
        var pasteboard = new Pasteboard
        {
            Id = pasteboardViewModel.Id,
            Name = pasteboardViewModel.Name,
            PasteboardFields = pasteboardFields
        };

        return pasteboard;
    }
}