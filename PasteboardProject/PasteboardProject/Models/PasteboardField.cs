namespace PasteboardProject.Models;

public class PasteboardField
{
    public int Id { get; set; }
    public string FieldName { get; set; }
    public string FieldValue { get; set; }
    public int PasteboardId { get; set; }
}