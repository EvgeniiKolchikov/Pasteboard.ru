using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;

namespace PasteboardProject.Interfaces;

public interface IVisitorRepository
{
    Task AddPasteboardVisitorToDataBase(PasteboardVisitor pasteboardVisitor);
    Task<List<PasteboardVisitorViewModel>> GetAllPasteboardVisitors(int pasteboardId);
}