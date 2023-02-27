using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;

namespace PasteboardProject.Interfaces;

public interface IUserRepository
{
    Task<UserViewModel> GetUserAsync(LoginViewModel loginViewModel);
    Task<bool> ExistUserInDataBaseAsync(RegisterViewModel registerViewModel);
    Task AddUserToDataBase(RegisterViewModel registerViewModel);
}