namespace PasteboardProject.Models.ViewModels;

public class PasteboardViewModel
{
    public Pasteboard Pasteboard { get; set; }
    
    public PasteboardViewModel()
    {
        Pasteboard = new Pasteboard();
    }
}