using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;

namespace PasteboardProject.Interfaces;

public interface IUserRepository
{
    Task<UserViewModel> GetUserAsync(LoginViewModel loginViewModel);
    Task<UserViewModel> GetUserAuthorizedAsync(string email);
    Task<bool> ExistUserInDataBaseAsync(RegisterViewModel registerViewModel);
    Task AddUserToDataBase(RegisterViewModel registerViewModel);
    Task<List<UsersListViewModel>> GetUserListAsync();
}