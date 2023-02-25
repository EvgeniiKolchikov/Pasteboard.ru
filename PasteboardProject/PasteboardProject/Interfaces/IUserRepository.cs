using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;

namespace PasteboardProject.Interfaces;

public interface IUserRepository
{
    Task<User> GetUserAsync(UserViewModel userViewModel);
    Task<bool> ExistUserInDataBaseAsync(UserViewModel userViewModel);
    Task AddUserToDataBase(User user);
}