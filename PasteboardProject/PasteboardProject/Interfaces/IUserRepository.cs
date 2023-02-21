using PasteboardProject.Models;

namespace PasteboardProject.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserAsync(string name, string password);
}