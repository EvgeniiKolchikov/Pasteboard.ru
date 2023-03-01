using System.ComponentModel.DataAnnotations;

namespace PasteboardProject.Models.ViewModels;

public class EditViewModel
{
    [Required(ErrorMessage = "Введите имя")]
    public string Name { get; set; }
    
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Введите старый пароль")]
    [MinLength(8,ErrorMessage = "Минимальная длина пароля 8")]
    public string OldPassword { get; set; }
    
    [Required(ErrorMessage = "Введите новый пароль")]
    [MinLength(8,ErrorMessage = "Минимальная длина пароля 8")]
    public string Password { get; set; }
    
    [Required]
    [Compare("Password",ErrorMessage = "Не совпадает с паролем")]
    public string ConfirmPassword { get; set; }
}