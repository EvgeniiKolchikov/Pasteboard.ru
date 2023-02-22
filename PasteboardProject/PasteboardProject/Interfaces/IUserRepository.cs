using PasteboardProject.Models;

namespace PasteboardProject.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserAsync(string name, string password);
    Task<bool> HasUserInDataBase(User user);
    Task AddUserToDataBase(User user);
}