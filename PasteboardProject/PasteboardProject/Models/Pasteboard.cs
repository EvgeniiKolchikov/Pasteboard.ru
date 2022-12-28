using System.ComponentModel.DataAnnotations;

namespace PasteboardProject.Models;

public class Pasteboard
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введите имя")] 
    [StringLength(50, ErrorMessage = "Длина строки должна быть до 50 символов")]
    public string Name { get; set; }
    public List<PasteboardField>? PasteboardFields { get; set; }
    
}