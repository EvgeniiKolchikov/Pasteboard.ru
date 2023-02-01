using Microsoft.AspNetCore.Mvc;
using PasteboardProject.Context;
using PasteboardProject.Exceptions;
using PasteboardProject.Interfaces;
using PasteboardProject.Models.Enums;
using PasteboardProject.Repositories;

namespace PasteboardProject.Controllers;

public class DiController : Controller
{
    private readonly ContextFactory _contextFactory;
    public DiController(ContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    [HttpGet]
    [Route("di/{id}/{repo}")]
    public async Task<IActionResult> ShowPasteboard(string id, RepositoriesEnum repo)
    {
        try
        {
            var repository = _contextFactory.GetRepository(Enum.GetName(repo));
            var pasteboardById = await repository.GetPasteboardByIdAsync(id);
            return Json(pasteboardById);
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
}