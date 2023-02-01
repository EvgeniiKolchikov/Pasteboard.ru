using PasteboardProject.Models.Enums;

namespace PasteboardProject.Models.ViewModels;

public class PasteboardViewModel
{
    public Pasteboard Pasteboard { get; set; }
    public string AspAction { get; set; }
    public List<ActivePasteboardField> ActivePasteboardFields { get; set; }
    public RepositoriesEnum RepositoriesEnum { get; set; }
}