using PasteboardProject.Models;

namespace PasteboardProject.Interfaces;

public interface IPasteboardRepository
{
    Task<Pasteboard> GetPasteboardByIdAsync(string id);
    Task SendPasteboardToDataBaseAsync(Pasteboard pasteboard, string userName);
    Task DeletePasteboard(Pasteboard pasteboard);
}