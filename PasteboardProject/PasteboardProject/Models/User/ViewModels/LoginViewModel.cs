using System.ComponentModel.DataAnnotations;
using PasteboardProject.Interfaces;

namespace PasteboardProject.Models.ViewModels;

public class LoginViewModel : ITokenGenerated
{
    [Required(ErrorMessage = "Введите адрес эл.почты")]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Неверный пароль")]
    public string Password { get; set; } = "";
}