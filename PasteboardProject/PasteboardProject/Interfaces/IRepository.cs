using PasteboardProject.Models;

namespace PasteboardProject.Interfaces;

public interface IRepository
{
    Task<Pasteboard> GetPasteboardById(string id);
    Task AddPasteboardAsync(Pasteboard pasteboard);

}