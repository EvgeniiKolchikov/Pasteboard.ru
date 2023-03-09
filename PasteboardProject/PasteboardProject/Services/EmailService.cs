using MailKit.Net.Smtp;
using MimeKit;
using PasteboardProject.Interfaces;
using static System.Int32;

namespace PasteboardProject.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        using var emailMessage = new MimeMessage();
 
        emailMessage.From.Add(new MailboxAddress("Регистрациа на сайте Pasteboard.ru", "registration@pasteboard.ru"));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message
        };

        using (var client = new SmtpClient())
        {
            var host = _configuration["MailSettings:Host"];
            TryParse(_configuration["MailSettings:Port"], out int myPort);
            var userName = _configuration["MailSettings:UserName"];
            var password = _configuration["MailSettings:Password"];
            
            await client.ConnectAsync(host, myPort, true);
            await client.AuthenticateAsync(userName, password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}
