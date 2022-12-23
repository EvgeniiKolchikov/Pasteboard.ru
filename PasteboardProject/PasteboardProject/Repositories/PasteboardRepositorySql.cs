using PasteboardProject.Interfaces;
using PasteboardProject.Models;

namespace PasteboardProject.Repositories;

public class PasteboardRepositorySql : IRepository
{
    public Pasteboard GetPasteboardById(int id)
    {
        throw new NotImplementedException();
    }

    public Task AddPasteboardAsync(Pasteboard pasteboard)
    {
        throw new NotImplementedException();
    }
}