using PasteboardProject.Interfaces;

namespace PasteboardProject.Models;

public class PasteboardVisitCounter
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Ip { get; set; }
    public string City { get; set; }
    public string JsonResponse { get; set; }
    public string UserAgentString { get; set; }
    public int PasteboardId { get; set; }
}