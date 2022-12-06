namespace PasteboardProject.Models;

public class Pasteboard
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<PasteboardField> PasteboardFields { get; set; }
}