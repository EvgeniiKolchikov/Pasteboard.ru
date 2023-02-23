using PasteboardProject.Models;

namespace PasteboardProject.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserAsync(string name);
    Task<bool> HasUserInDataBase(User user);
    Task AddUserToDataBase(User user);
    Task<bool> CheckUserNameAndPassword(User user);
}