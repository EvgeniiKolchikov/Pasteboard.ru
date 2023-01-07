using System.Globalization;
using Microsoft.EntityFrameworkCore;
using PasteboardProject.Context;
using PasteboardProject.Interfaces;
using PasteboardProject.Models;

namespace PasteboardProject.Repositories;

public class PasteboardRepositorySql : IRepository
{
    private ApplicationContext _db;
    public PasteboardRepositorySql(ApplicationContext context)
    {
        _db = context;
    }
    public async Task<Pasteboard> GetPasteboardById(string id)
    {
        var isInt = int.TryParse(id, out var intId);
        if (isInt)
        {
            var pasteboard = await _db.Pasteboards.FirstOrDefaultAsync(p => p.Id == intId);
            var pasteboardField = _db.PasteboardFields.Where(pf => pf.PasteboardId == intId).ToList();
            if (pasteboard is null) return new Pasteboard();
            pasteboard.PasteboardFields = pasteboardField;
            return pasteboard;
        }
        else
        {
            var pasteboard = await _db.Pasteboards.FirstOrDefaultAsync(p => p.Name == id);
            if (pasteboard is null) return new Pasteboard();
            pasteboard.PasteboardFields = _db.PasteboardFields.Where(pf => pf.PasteboardId == pasteboard.Id).ToList();
            return pasteboard;
        }
    }

    public async Task AddPasteboardAsync(Pasteboard pasteboard)
    {
        var pasteboardInDataBase = await _db.Pasteboards.FirstOrDefaultAsync(p => p.Id == pasteboard.Id);
        if (pasteboardInDataBase is null)
        {
            _db.Pasteboards.Add(pasteboard);
            await _db.SaveChangesAsync();
        }
        else
        {
            foreach (var pf in pasteboard.PasteboardFields)
            {
                pf.PasteboardId = pasteboard.Id;
            }
            var pasteboardFieldsForRemove = _db.PasteboardFields.Where(pf => pf.PasteboardId == pasteboard.Id);
            _db.PasteboardFields.RemoveRange(pasteboardFieldsForRemove);
            _db.PasteboardFields.AddRange(pasteboard.PasteboardFields);
            await _db.SaveChangesAsync();
        }
    }
}