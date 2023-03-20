using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;

namespace PasteboardProject.Interfaces;

public interface IVisitorRepository
{
    Task<List<PasteboardVisitorViewModel>> GetAllPasteboardVisitors(int pasteboardId);
    Task AddPasteboardVisitorToDataBase(string ipAdress, string city, string userAgent, int pasteboardId);
}