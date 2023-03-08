using System.ComponentModel.DataAnnotations;

namespace PasteboardProject.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationDateTime { get; set; }
    public DateTime LastVisitDateTime { get; set; }
}