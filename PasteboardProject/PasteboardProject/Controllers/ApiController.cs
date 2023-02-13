using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Fluent;
using PasteboardProject.Exceptions;
using PasteboardProject.Interfaces;
using PasteboardProject.Models;

namespace PasteboardProject.api;

[Route("[controller]")]
public class ApiController : Controller
{
    private readonly IRepository _repository;
    private static readonly Logger Logger = LogManager.GetLogger("ApiController");
    public ApiController(IRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [Route("{id}")]
    [Route("pasteboard/{id}")]
    public async Task GetPasteboardById(string id)
    {
        Logger.Debug($"Getpasteboard{id} Action");
        try
        {
            var pasteboardById = await _repository.GetPasteboardByIdAsync(id);
            await HttpContext.Response.WriteAsJsonAsync(pasteboardById);
        }
        catch (CustomException e)
        {
            Logger.Warn($"\n*Message: {e.Message} \n*Data:{e.Data} \n*StackTrace:{e.StackTrace}");
            await HttpContext.Response.WriteAsJsonAsync(e.Message);
        }
        catch (Exception e)
        {
            Logger.Error($"\n*Message: {e.Message} \n*Data:{e.Data} \n*StackTrace:{e.StackTrace}");
            await HttpContext.Response.WriteAsJsonAsync(e.Message);
        }
    }
    
    [HttpPost]
    [Route("pasteboard/create")]
    public async Task CreatePasteboard([FromBody]Pasteboard pasteboard)
    {
        Logger.Debug("CreatePasteboard Action");
        try
        {
            await _repository.SendPasteboardToDataBaseAsync(pasteboard);
            await HttpContext.Response.WriteAsJsonAsync(pasteboard);
        }
        catch (Exception e)
        {
            Logger.Error($"\n*Message: {e.Message} \n*Data:{e.Data} \n*StackTrace:{e.StackTrace}");
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(new { message = "Некорректные данные" });
        }
    }

    [HttpPost]
    [Route("pasteboard/edit/{id}")]
    public async Task EditPasteboard([FromBody]Pasteboard pasteboard)
    {
        Logger.Debug("EditPasteboard Action");
        try
        {
            await _repository.SendPasteboardToDataBaseAsync(pasteboard);
            await HttpContext.Response.WriteAsJsonAsync(pasteboard);
        }
        catch (CustomException e)
        {
            Logger.Warn($"\n*Message: {e.Message} \n*Data:{e.Data} \n*StackTrace:{e.StackTrace}");
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(e.Message);
        }
        catch (Exception e)
        {
            Logger.Error($"\n*Message: {e.Message} \n*Data:{e.Data} \n*StackTrace:{e.StackTrace}");
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(new { message = "Некорректные данные" });
        }
    }
}