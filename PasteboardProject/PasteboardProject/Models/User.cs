using System.ComponentModel.DataAnnotations;

namespace PasteboardProject.Models;

public class User
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required(ErrorMessage = "Неверный пароль")]
    public string Password { get; set; }
    public string Email { get; set; }
    public List<Pasteboard>? Pasteboards { get; set; }
}