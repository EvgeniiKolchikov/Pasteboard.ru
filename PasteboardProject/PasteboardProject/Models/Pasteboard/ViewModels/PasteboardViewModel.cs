using System.ComponentModel.DataAnnotations;

namespace PasteboardProject.Models.ViewModels;

public class PasteboardViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введите имя")] 
    [StringLength(50, ErrorMessage = "Длина строки должна быть до 50 символов")]
    public string Name { get; set; }
    public string AspAction { get; set; }
    public List<ActivePasteboardField> ActivePasteboardFields { get; set; }

}