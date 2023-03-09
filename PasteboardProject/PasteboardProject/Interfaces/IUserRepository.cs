using PasteboardProject.Models;
using PasteboardProject.Models.ViewModels;

namespace PasteboardProject.Interfaces;

public interface IUserRepository
{
    Task<UserViewModel> GetUserViewModelLoginAsync(LoginViewModel loginViewModel);
    Task<UserViewModel> GetUserViewModelAuthorizedAsync(string email);
    Task<bool> ExistUserInDataBaseAsync(RegisterViewModel registerViewModel);
    Task AddUserToDataBaseAsync(RegisterViewModel registerViewModel);
    Task UpdateUserAsync(EditViewModel editViewModel);
    Task<List<UsersListViewModel>> GetUserListAsync();
    Task<bool> UserTokenConfirmation(string token);
    Task<string> GetUserToken(string userEmail);
    Task<bool> EmailConfirmedCheck(string userEmail);
}