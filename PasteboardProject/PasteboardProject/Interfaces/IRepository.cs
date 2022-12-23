using PasteboardProject.Models;

namespace PasteboardProject.Interfaces;

public interface IRepository
{
    Pasteboard GetPasteboardById(int id);
    Task AddPasteboardAsync(Pasteboard pasteboard);

}