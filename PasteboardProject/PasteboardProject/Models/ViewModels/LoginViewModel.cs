using System.ComponentModel.DataAnnotations;

namespace PasteboardProject.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Введите адрес эл.почты")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Неверный пароль")]
    public string Password { get; set; }
}