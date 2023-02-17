namespace PasteboardProject.Models;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public List<Pasteboard>? UserPasteboards { get; set; }
}