using System.Security.Cryptography;
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

    public async Task<User> GetUserAsync(string name)
    {
        var user = _db.Users.FirstOrDefaultAsync(u => u.Name == name).Result;
        if (user == null) throw new NullReferenceException("Не найден пользователь");
        var pasteboards = await _db.Pasteboards.Where(p => p.UserId == user.Id).ToListAsync();
        user.Pasteboards = pasteboards;
        return user;
    }

    public async Task<bool> ExistUserInDataBaseAsync(User user)
    {
        return await _db.Users.AnyAsync(u => u.Name == user.Name);
    }

    public async Task<bool> CheckUserNameAndPasswordAsync(User user)
    {
        var userDb = await _db.Users.FirstOrDefaultAsync(u => u.Name == user.Name);
        if (userDb != null)
        {
            return VerifyHashedPassword(userDb.Password, user.Password);
        }

        throw new Exception("Отсутсвует в базе");
    }
    
    public async Task AddUserToDataBase(User user)
    {
        user.Password = HashPassword(user.Password);
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }

    private string HashPassword(string password)
    {
        byte[] salt;
        byte[] buffer2;
        if (password == null)
        {
            throw new ArgumentNullException("password");
        }
        using (var bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
        {
            salt = bytes.Salt;
            buffer2 = bytes.GetBytes(0x20);
        }
        byte[] dst = new byte[0x31];
        Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
        Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
        return Convert.ToBase64String(dst);
    }
    
    private bool VerifyHashedPassword(string hashedPassword, string password)
    {
        byte[] buffer4;
        if (hashedPassword == null)
        {
            return false;
        }
        if (password == null)
        {
            throw new ArgumentNullException("password");
        }
        byte[] src = Convert.FromBase64String(hashedPassword);
        if ((src.Length != 0x31) || (src[0] != 0))
        {
            return false;
        }
        byte[] dst = new byte[0x10];
        Buffer.BlockCopy(src, 1, dst, 0, 0x10);
        byte[] buffer3 = new byte[0x20];
        Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
        using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
        {
            buffer4 = bytes.GetBytes(0x20);
        }
        return ByteArraysEqual(buffer3, buffer4);
    }
    
    private bool ByteArraysEqual(byte[] b1, byte[] b2)
    {
        if (b1 == b2) return true;
        if (b1 == null || b2 == null) return false;
        if (b1.Length != b2.Length) return false;
        for (int i = 0; i < b1.Length; i++)
        {
            if (b1[i] != b2[i]) return false;
        }
        return true;
    }
 
}