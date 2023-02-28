using System.ComponentModel.DataAnnotations;

namespace PasteboardProject.Models;

public class PasteboardField
{
    public int Id { get; set; }
    [Required] public string FieldName { get; set; } = "";
    [Required] public string FieldValue { get; set; } = "";
    public int PasteboardId { get; set; }
}