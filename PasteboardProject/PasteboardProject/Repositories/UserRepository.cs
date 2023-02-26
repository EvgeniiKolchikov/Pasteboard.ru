using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using NLog;
using PasteboardProject.Context;
using PasteboardProject.Exceptions;
using PasteboardProject.Interfaces;
using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;

namespace PasteboardProject.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _db;
    private static readonly Logger Logger = LogManager.GetLogger("User Repository");
    private const string Salt = "2E7A44BC-0377-4BD0-BB88-EA806080DDBB";
    public UserRepository(ApplicationContext context)
    {
        _db = context;
        Logger.Debug("User Repository in");
    }

    public async Task<User> GetUserAsync(UserViewModel userViewModel)
    {
        var hashPassword = GetHashPassword(userViewModel.Name, userViewModel.Password);
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Name == userViewModel.Name && u.Password == hashPassword);
        if (user == null) throw new CustomException(CustomException.DefaultMessage);
        user.Pasteboards = await _db.Pasteboards.Where(p => p.UserId == user.Id).ToListAsync();
        return user;
    }
    
    public async Task<bool> ExistUserInDataBaseAsync(UserViewModel userViewModel)
    {
        return await _db.Users.AnyAsync(u => u.Name == userViewModel.Name);
    }
    
    public async Task AddUserToDataBase(User user)
    {
        user.Password = GetHashPassword(user.Name, user.Password);
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }

    private string GetHashPassword(string userName, string password)
    { 
        var passwordHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password,
            Encoding.ASCII.GetBytes(Salt + userName),        
            KeyDerivationPrf.HMACSHA256,        
            148, 410 / 7));
        return passwordHashed;
    }

   
}