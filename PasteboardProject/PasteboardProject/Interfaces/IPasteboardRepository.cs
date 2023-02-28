using PasteboardProject.Models;

namespace PasteboardProject.Interfaces;

public interface IPasteboardRepository
{
    Task<Pasteboard> GetPasteboardByIdAsync(string id);
    Task<Pasteboard> GetPasteboardByIdWithUserCheckAsync(string id, string userEmail);
    Task SendPasteboardToDataBaseAsync(Pasteboard pasteboard, string userEmail);
    Task DeletePasteboard(Pasteboard pasteboard);
}