using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
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

    public async Task<UserViewModel> GetUserAsync(LoginViewModel loginViewModel)
    {
        var hashPassword = GetHashPassword(loginViewModel.Email, loginViewModel.Password);
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == loginViewModel.Email && u.Password == hashPassword);
        if (user == null) throw new CustomException(CustomException.DefaultMessage);
        var userViewModel = new UserViewModel
        {
            Name = user.Name,
            Email = user.Email,
            Pasteboards = await _db.Pasteboards.Where(p => p.UserId == user.Id).ToListAsync()
        };
        return userViewModel;
    }

    public async Task<UserViewModel> GetUserAuthorizedAsync(string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) throw new CustomException(CustomException.DefaultMessage);
        var userViewModel = new UserViewModel
        {
            Name = user.Name,
            Email = user.Email,
            Pasteboards = await _db.Pasteboards.Where(p => p.UserId == user.Id).ToListAsync()
        };
        return userViewModel;
    }

    public async Task<bool> ExistUserInDataBaseAsync(RegisterViewModel registerViewModel)
    {
        return await _db.Users.AnyAsync(u => u.Email == registerViewModel.Email && u.Name == registerViewModel.Name);
    }
    
    public async Task AddUserToDataBase(RegisterViewModel registerViewModel)
    {
        var user = new User
        {
            Name = registerViewModel.Name,
            Email = registerViewModel.Email,
            Password = GetHashPassword(registerViewModel.Email, registerViewModel.Password),
            RegistrationDateTime = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
            LastVisitDateTime = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
        };
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }

    public async Task<List<UsersListViewModel>> GetUserListAsync()
    {
        return await _db.Users.Select(u => new UsersListViewModel
        {
            Email = u.Email,
            RegistrationDate = u.RegistrationDateTime,
            LastVisitDate = u.LastVisitDateTime
        }).ToListAsync();
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