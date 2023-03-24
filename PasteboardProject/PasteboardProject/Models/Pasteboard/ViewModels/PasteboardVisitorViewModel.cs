namespace PasteboardProject.Models.ViewModels;

public class PasteboardVisitorViewModel
{
    public string PasteboardName { get; set; } = "";
    public DateTime DateTime { get; set; }
    public string Ip { get; set; } = "";
    public string City { get; set; } = "";
    public string UserAgent { get; set; } = "";
}