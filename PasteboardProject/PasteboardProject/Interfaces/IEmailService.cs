namespace PasteboardProject.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string email, string emailToken);
}