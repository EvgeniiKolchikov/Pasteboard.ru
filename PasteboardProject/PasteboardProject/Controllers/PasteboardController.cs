using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Fluent;
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
        Log.Debug($"Метод Show Pasteboard index = {id}");
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
    public IActionResult CreatePasteboardAsync()
    {
        Logger.Debug($"This is CreatePasteboardAsync Action: Get");
        var activePasteboardFields = new List<ActivePasteboardField>();
        var pasteboardViewModel = new PasteboardViewModel()
        {
            AspAction = "CreatePasteboardAsync",
            ActivePasteboardFields = AddEmptyFields(activePasteboardFields)
        };
        return View("CreateEditPasteboard",pasteboardViewModel);
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreatePasteboardAsync(PasteboardViewModel pasteboardViewModel)
    {
        Logger.Debug($"This is CreatePasteboardAsync Action: Post");
        var pasteboard = DeleteEmptyFields(pasteboardViewModel);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        
        await _pasteboardRepository.SendPasteboardToDataBaseAsync(pasteboard, userEmail);
        return View("ShowPasteboard", pasteboard);
    }
    
    [HttpGet]
    [Route("edit/{id}")]
    public async Task<IActionResult> EditPasteboardAsync(string id)
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
    [Route("edit/")]
    public async Task<IActionResult> EditPasteboardAsync(PasteboardViewModel pasteboardViewModel)
    {
        try
        {
            Logger.Debug($"This is EditPasteboard Action: Post");
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var pasteboard = DeleteEmptyFields(pasteboardViewModel);
            await _pasteboardRepository.SendPasteboardToDataBaseAsync(pasteboard, userEmail);
            var id = pasteboard.Id;
            return RedirectToAction("ShowPasteboard", new{id});
        }
        catch (Exception e)
        {
            Logger.Error($"Exception: {e.Message} {e.Data} {e.StackTrace}");
            return View("~/Views/Error/ErrorPage.cshtml", e.Message);
        }
    }
    
    [HttpGet("delete")]
    public async Task<IActionResult> DeletePasteboardAsync(string id)
    {
        try
        {
            Log.Debug("This is DeletePasteboard Action: Get");
            var pasteboard = await _pasteboardRepository.GetPasteboardByIdAsync(id);
            return View("DeletePasteboard",pasteboard);
        }
        catch (Exception e)
        {
            Logger.Error($"Exception: {e.Message} {e.Data} {e.StackTrace}");
            return View("~/Views/Error/ErrorPage.cshtml", e.Message);
        }
    }
    
    [HttpPost("delete")]
    public async Task<IActionResult> DeletePasteboardAsync(Pasteboard pasteboard)
    {
        Log.Debug("This is DeletePasteboard Action: Post");
        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            await _pasteboardRepository.DeletePasteboardAsync(pasteboard,userEmail);
            return RedirectToAction("UserPage", "User");
        }
        catch (Exception e)
        {
            return View("~/Views/Error/ErrorPage.cshtml", e.Message);
        }
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