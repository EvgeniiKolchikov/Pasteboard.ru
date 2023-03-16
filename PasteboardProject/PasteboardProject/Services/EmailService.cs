using MailKit.Net.Smtp;
using MimeKit;
using PasteboardProject.Interfaces;
using static System.Int32;

namespace PasteboardProject.Services;

public class EmailService : IEmailService
{
    private readonly string _name = "Регистрациа на сайте Pasteboard.ru";
    private readonly string _subject = "Регистрация нового пользователя";
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task SendEmailAsync(string email, string emailToken)
    {
        var _emailAddress = _configuration["MailSettings:From"];
        using var emailMessage = new MimeMessage();
 
        emailMessage.From.Add(new MailboxAddress(_name, _emailAddress));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = _subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $"Перейдите по ссылке для активации пользователя: <a href=\"http://pasteboard.ru/user/verify/{emailToken}\">Активировать пользователя</a>"
        };

        using var client = new SmtpClient();
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
