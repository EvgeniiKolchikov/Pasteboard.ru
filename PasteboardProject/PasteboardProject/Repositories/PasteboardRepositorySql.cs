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
    
    public Pasteboard GetPasteboardById(int id)
    {
         var pasteboard = _db.Pasteboards.FirstAsync(p => p.Id == id).Result;
         var pasteboardField = _db.PasteboardFields.Where(pf => pf.PasteboardId == id).ToList();
         pasteboard.PasteboardFields = pasteboardField;
         return pasteboard;
    }

    public async Task AddPasteboardAsync(Pasteboard pasteboard)
    {
        var pasteboardExist = await _db.Pasteboards.AnyAsync(p => p.Id == pasteboard.Id);
        //firstofdefault()
        if (!pasteboardExist) //id == 0
        {
            _db.Pasteboards.Add(pasteboard);
            await _db.SaveChangesAsync();
        }
        else
        {
            var oldFields = _db.PasteboardFields.Where(pf => pf.PasteboardId == pasteboard.Id);
            foreach (var pf in oldFields)
            {
                pf.PasteboardId = null;
            }
            _db.PasteboardFields.UpdateRange(oldFields);
            await _db.SaveChangesAsync();
            _db.Pasteboards.Update(pasteboard);
            _db.PasteboardFields.AddRange(pasteboard.PasteboardFields);
            await _db.SaveChangesAsync();
        }
    }
}