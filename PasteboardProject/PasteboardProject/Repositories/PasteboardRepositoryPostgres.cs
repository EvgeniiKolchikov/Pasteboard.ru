using Microsoft.EntityFrameworkCore;
using NLog;
using PasteboardProject.Context;
using PasteboardProject.Exceptions;
using PasteboardProject.Interfaces;
using PasteboardProject.Models;

namespace PasteboardProject.Repositories;

public class PasteboardRepositoryPostgres : IPasteboardRepository
{
    private readonly ApplicationContext _db;
    private static readonly Logger Logger = LogManager.GetLogger("PasteboardRepositoryPostgres");
    public PasteboardRepositoryPostgres(ApplicationContext context)
    {
        _db = context;
        Logger.Debug("PostgresRepository Init");
    }
    public async Task<Pasteboard> GetPasteboardByIdAsync(string id)
    {
        Logger.Debug("Method GetPasteboardByIdAsync");
        var isInt = int.TryParse(id, out var intId);
        if (isInt)
        {
            var pasteboard = await _db.Pasteboards.FirstOrDefaultAsync(p => p.Id == intId);
            if (pasteboard is null)
            {
                throw new CustomException(CustomException.NotFoundMessage);
            }
            pasteboard.PasteboardFields = _db.PasteboardFields.Where(pf => pf.PasteboardId == intId).ToList();
            return pasteboard;
        }
        else
        {
            var pasteboard = await _db.Pasteboards.FirstOrDefaultAsync(p => p.Name == id);
            if (pasteboard is null)
            {
                throw new CustomException(CustomException.NotFoundMessage);
            }
            pasteboard.PasteboardFields = _db.PasteboardFields.Where(pf => pf.PasteboardId == pasteboard.Id).ToList();
            return pasteboard;
        }
    }

    public async Task SendPasteboardToDataBaseAsync(Pasteboard pasteboard, string userName)
    {
        try
        {
            Logger.Debug("Method SendPasteboardToDataBaseAsync");
            var userId = _db.Users.FirstOrDefault(u => u.Name == userName).Id;
            pasteboard.UserId = userId;
            var pasteboardInDataBase = await _db.Pasteboards.FirstOrDefaultAsync(p => p.Id == pasteboard.Id);
            if (pasteboardInDataBase is null)
            {
                await AddToDataBaseAsync(pasteboard);
            }
            else
            {
                await EditPasteboardInDataBase(pasteboardInDataBase, pasteboard);
            }
        }
        catch (Exception e)
        {
            throw new Exception("ошибка");
        }
    }

    private async Task AddToDataBaseAsync(Pasteboard pasteboard)
    {
        Logger.Debug("Create New Pasteboard");
        await _db.Pasteboards.AddAsync(pasteboard);
        await _db.SaveChangesAsync();
    }

    private async Task EditPasteboardInDataBase(Pasteboard pasteboardInDataBase, Pasteboard pasteboardForEdit)
    {
        Logger.Debug($"Edit Pasteboard {pasteboardInDataBase.Name}");
        pasteboardInDataBase.Name = pasteboardForEdit.Name;
        _db.Pasteboards.Update(pasteboardInDataBase);
        foreach (var pf in pasteboardForEdit.PasteboardFields)
        {
            pf.PasteboardId = pasteboardForEdit.Id;
        }
        var pasteboardFieldsForRemove = _db.PasteboardFields.Where(pf => pf.PasteboardId == pasteboardForEdit.Id);
        _db.PasteboardFields.RemoveRange(pasteboardFieldsForRemove);
        _db.PasteboardFields.AddRange(pasteboardForEdit.PasteboardFields);
        await _db.SaveChangesAsync();
    }
}