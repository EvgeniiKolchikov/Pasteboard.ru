using Microsoft.AspNetCore.Mvc;
using PasteboardProject.Exceptions;
using PasteboardProject.Interfaces;
using PasteboardProject.Models;

namespace PasteboardProject.api;

public class ApiController : Controller
{
    private IRepository _repository;

    public ApiController(IRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task GetPasteboardById(string id)
    {
        try
        {
            var pasteboardById = await _repository.GetPasteboardByIdAsync(id);
            await HttpContext.Response.WriteAsJsonAsync(pasteboardById);
        }
        catch (CustomException e)
        {
            await HttpContext.Response.WriteAsJsonAsync(e.Message);
        }
        catch (Exception e)
        {
            await HttpContext.Response.WriteAsJsonAsync(e.Message);
        }
    }
    
    [HttpPost]
    public async Task CreatePasteboard([FromBody]Pasteboard pasteboard)
    {
        try
        {
            await _repository.AddPasteboardAsync(pasteboard);
            await HttpContext.Response.WriteAsJsonAsync(pasteboard);
        }
        catch (Exception)
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(new { message = "Некорректные данные" });
        }
    }

    [HttpPost]
    public async Task EditPasteboard([FromBody]Pasteboard pasteboard)
    {
        try
        {
            await _repository.AddPasteboardAsync(pasteboard);
            await HttpContext.Response.WriteAsJsonAsync(pasteboard);
        }
        catch (CustomException e)
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(e.Message);
        }
        catch (Exception)
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsJsonAsync(new { message = "Некорректные данные" });
        }
    }
}