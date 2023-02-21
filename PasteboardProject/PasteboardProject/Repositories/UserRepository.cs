using Microsoft.EntityFrameworkCore;
using NLog;
using PasteboardProject.Context;
using PasteboardProject.Interfaces;
using PasteboardProject.Models;

namespace PasteboardProject.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _db;
    private static readonly Logger Logger = LogManager.GetLogger("User Repository");

    public UserRepository(ApplicationContext context)
    {
        _db = context;
        Logger.Debug("User Repository in");
    }

    public async Task<User> GetUserAsync(string name, string password)
    {
        var hashPassword = GetHashPassword(password);
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Name == name && u.Password == hashPassword);
        if (user is null)
        {
            throw new NullReferenceException("User Not Found");
        }

        return user;
    }

    private string GetHashPassword(string password)
    {
        //hash logic
        return password;
    }

 
}