using System.ComponentModel.DataAnnotations;
using PasteboardProject.Interfaces;

namespace PasteboardProject.Models.ViewModels;

public class RegisterViewModel : ITokenGenerated
{
    [Required(ErrorMessage = "Введите имя")]
    public string Name { get; set; } = "";
    [Required(ErrorMessage = "Введите адрес эл.почты")]
    public string Email { get; set; } = "";
    [Required(ErrorMessage = "Введите пароль")]
    [MinLength(8,ErrorMessage = "Минимальная длина пароля 8")]
    public string Password { get; set; } = "";
    [Required]
    [Compare("Password",ErrorMessage = "Не совпадает с паролем")]
    public string ConfirmPassword { get; set; } = "";
    
}