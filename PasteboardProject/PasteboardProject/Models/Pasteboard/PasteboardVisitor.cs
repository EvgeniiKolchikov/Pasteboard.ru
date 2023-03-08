using System.ComponentModel.DataAnnotations;
using PasteboardProject.Interfaces;

namespace PasteboardProject.Models;

public class PasteboardVisitor
{
    [Key]
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Ip { get; set; }
    public string City { get; set; }
    public string UserAgent { get; set; }
    public int PasteboardId { get; set; }
}