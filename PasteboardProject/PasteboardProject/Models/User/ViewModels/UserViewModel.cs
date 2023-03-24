namespace PasteboardProject.Models.ViewModels;

public class UserViewModel
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public List<Pasteboard> Pasteboards { get; set; }
}