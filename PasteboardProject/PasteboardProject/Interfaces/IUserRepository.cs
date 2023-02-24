using PasteboardProject.Models;

namespace PasteboardProject.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserAsync(string name);
    Task<bool> ExistUserInDataBaseAsync(User user);
    Task AddUserToDataBase(User user);
    Task<bool> CheckUserNameAndPasswordAsync(User user);
}