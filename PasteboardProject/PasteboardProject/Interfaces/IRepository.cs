using PasteboardProject.Models;

namespace PasteboardProject.Interfaces;

public interface IRepository
{
    Task<Pasteboard> GetPasteboardByIdAsync(string id);
    Task SendPasteboardToDataBaseAsync(Pasteboard pasteboard);

}