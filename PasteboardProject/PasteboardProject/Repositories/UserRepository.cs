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

    public async Task<UserViewModel> GetUserViewModelLoginAsync(LoginViewModel loginViewModel)
    {
        var hashPassword = GetHashPassword(loginViewModel.Email, loginViewModel.Password);
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == loginViewModel.Email && u.Password == hashPassword);
        if (user == null) throw new CustomException(CustomException.DefaultMessage);
        user.LastVisitDateTime = DateTime.UtcNow;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        var userViewModel = new UserViewModel
        {
            Name = user.Name,
            Email = user.Email,
            Pasteboards = await _db.Pasteboards.Where(p => p.UserId == user.Id).ToListAsync()
        };
        return userViewModel;
    }

    public async Task<UserViewModel> GetUserViewModelAuthorizedAsync(string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) throw new CustomException(CustomException.DefaultMessage);
        var userViewModel = new UserViewModel
        {
            Name = user.Name,
            Email = user.Email,
            Pasteboards = await _db.Pasteboards.Where(p => p.UserId == user.Id && p.IsDeleted == false).ToListAsync()
        };
        return userViewModel;
    }

    public async Task<bool> ExistUserInDataBaseAsync(RegisterViewModel registerViewModel)
    {
        return await _db.Users.AnyAsync(u => u.Email == registerViewModel.Email);
    }
    
    public async Task AddUserToDataBaseAsync(RegisterViewModel registerViewModel)
    {
        var user = new User
        {
            Name = registerViewModel.Name,
            Email = registerViewModel.Email,
            Password = GetHashPassword(registerViewModel.Email, registerViewModel.Password),
            RegistrationDateTime = DateTime.UtcNow,
            LastVisitDateTime = DateTime.UtcNow,
            EmailConfirmationToken = CreateUserToken(registerViewModel.Email),
            ConfirmedEmail = false
        };
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(EditViewModel editViewModel)
    {
        var hashOldPassword = GetHashPassword(editViewModel.Email, editViewModel.OldPassword);
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Email == editViewModel.Email && u.Password == hashOldPassword);
        if (user == null) throw new CustomException(CustomException.DefaultMessage);
        var hashNewPassword = GetHashPassword(editViewModel.Email, editViewModel.Password);
        user.Name = editViewModel.Name;
        user.Password = hashNewPassword;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task<List<UsersListViewModel>> GetUserListAsync()
    {
        return await _db.Users.Select(u => new UsersListViewModel
        {
            Email = u.Email,
            RegistrationDate = u.RegistrationDateTime.ToLocalTime(),
            LastVisitDate = u.LastVisitDateTime.ToLocalTime()
        }).ToListAsync();
    }

    public async Task<string> GetUserToken(string userEmail)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) throw new CustomException(CustomException.UserNotFoundMessage);
        return user.EmailConfirmationToken;
    }

    public async Task<bool> EmailConfirmedCheck(string userEmail)
    {
       var emailConfirmed = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail && u.ConfirmedEmail == true);
       if (emailConfirmed == null) return false;
       return true;
    }
    public async Task<bool> UserTokenConfirmation(string token)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
        if (user == null) return false;
        user.ConfirmedEmail = true;
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return true;
    }

    private string GetHashPassword(string userEmail, string password)
    { 
        var passwordHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password,
            Encoding.ASCII.GetBytes(Salt + userEmail),        
            KeyDerivationPrf.HMACSHA256,        
            148, 410 / 7));
        return passwordHashed;
    }

    private string CreateUserToken(string userEmail)
    {
        return  Convert.ToBase64String(KeyDerivation.Pbkdf2(userEmail,
            Encoding.ASCII.GetBytes(Salt),        
            KeyDerivationPrf.HMACSHA256,        
            148, 410 / 7));
    }
    
}