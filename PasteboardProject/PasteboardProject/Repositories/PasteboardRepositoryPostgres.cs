using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Fluent;
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
                throw new CustomException(CustomException.PasteboardNotFoundMessage);
            }
            pasteboard.PasteboardFields = _db.PasteboardFields.Where(pf => pf.PasteboardId == intId).ToList();
            return pasteboard;
        }
        else
        {
            var pasteboard = await _db.Pasteboards.FirstOrDefaultAsync(p => p.Name == id);
            if (pasteboard is null)
            {
                throw new CustomException(CustomException.PasteboardNotFoundMessage);
            }
            pasteboard.PasteboardFields = _db.PasteboardFields.Where(pf => pf.PasteboardId == pasteboard.Id).ToList();
            return pasteboard;
        }
    }

    public async Task<Pasteboard> GetPasteboardByIdWithUserCheckAsync(string id, string userEmail)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        var isInt = int.TryParse(id, out var intId);
        if (isInt)
        {
            var pasteboard = await _db.Pasteboards.FirstOrDefaultAsync(p => p.Id == intId);
            if (pasteboard.UserId != user.Id) throw new CustomException(CustomException.AccessDeniedMessage);
            if (pasteboard is null) throw new CustomException(CustomException.PasteboardNotFoundMessage);
            pasteboard.PasteboardFields = _db.PasteboardFields.Where(pf => pf.PasteboardId == intId).ToList();
            return pasteboard;
        }
        else
        {
            var pasteboard = await _db.Pasteboards.FirstOrDefaultAsync(p => p.Name == id);
            if (pasteboard is null)
            {
                throw new CustomException(CustomException.PasteboardNotFoundMessage);
            }
            pasteboard.PasteboardFields = _db.PasteboardFields.Where(pf => pf.PasteboardId == pasteboard.Id).ToList();
            return pasteboard;
        }
    }

    public async Task SendPasteboardToDataBaseAsync(Pasteboard pasteboard, string userEmail)
    {
        Logger.Debug("Method SendPasteboardToDataBaseAsync");
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) throw new CustomException(CustomException.UserNotFoundMessage);
        pasteboard.UserId = user.Id;
        var pasteboardInDataBase = await _db.Pasteboards.FirstOrDefaultAsync(p => p.Id == pasteboard.Id);
        if (pasteboardInDataBase is null)
        {
            await AddToDataBaseAsync(pasteboard);
        }
        else
        {
            await EditPasteboardInDataBaseAsync(pasteboardInDataBase, pasteboard);
        }
    }

    public async Task DeletePasteboardAsync(Pasteboard pasteboard, string userEmail)
    {
        Log.Debug($"Deleted Pasteboard {pasteboard?.Id} - {pasteboard?.Name}");
        var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
        if ( user != null && pasteboard.UserId == user.Id)
        {
            pasteboard.IsDeleted = true;
            _db.Pasteboards.Update(pasteboard);
            await _db.SaveChangesAsync();
        }
        else
        {
            throw new CustomException(CustomException.PasteboardNotFoundMessage);
        }
    }

    private async Task AddToDataBaseAsync(Pasteboard pasteboard)
    {
        Logger.Debug("Create New Pasteboard");
        await _db.Pasteboards.AddAsync(pasteboard);
        await _db.SaveChangesAsync();
    }

    private async Task EditPasteboardInDataBaseAsync(Pasteboard pasteboardInDataBase, Pasteboard pasteboardForEdit)
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