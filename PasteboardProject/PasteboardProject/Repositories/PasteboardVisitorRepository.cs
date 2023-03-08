using Microsoft.EntityFrameworkCore;
using PasteboardProject.Context;
using PasteboardProject.Exceptions;
using PasteboardProject.Interfaces;
using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;

namespace PasteboardProject.Repositories;

public class PasteboardVisitorRepository : IVisitorRepository
{
    private readonly ApplicationContext _db;
    public PasteboardVisitorRepository(ApplicationContext context)
    {
        _db = context;
    }

    public async Task AddPasteboardVisitorToDataBase(PasteboardVisitor pasteboardVisitor)
    {
        await _db.AddAsync(pasteboardVisitor);
        await _db.SaveChangesAsync();
    }

    public async Task<List<PasteboardVisitorViewModel>> GetAllPasteboardVisitors(int pasteboardId)
    {
        var pasteboard = _db.Pasteboards.FirstOrDefault(p => p.Id == pasteboardId);
        var visitors = await _db.PasteboardVisitors.Where(v => v.PasteboardId == pasteboardId).ToListAsync();
        var visitorViewModel = visitors.Select(visitor => new PasteboardVisitorViewModel {PasteboardName = pasteboard.Name, City = visitor.City, DateTime = visitor.DateTime.ToLocalTime(), Ip = visitor.Ip, UserAgent = visitor.UserAgent }).ToList();
        if (pasteboard is null || visitors is null)
        {
            throw new CustomException(CustomException.PasteboardNotFoundMessage);
        }
        return visitorViewModel;
    }
}