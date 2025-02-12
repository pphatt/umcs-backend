using Microsoft.Extensions.Options;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Contracts.Common.Email;
using System.Net;
using System.Net.Mail;

namespace Server.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _emailSettings = settings.Value;
    }

    public async Task SendEmailAsync(MailRequest email)
    {
        using var client = new SmtpClient(_emailSettings.Host);

        var emailMessage = new MailMessage()
        {
            From = new MailAddress(_emailSettings.Email),
            Subject = email.Subject,
            Body = email.Body,
            IsBodyHtml = true,
        };

        emailMessage.To.Add(new MailAddress(email.ToEmail));

        client.Host = _emailSettings.Host;
        client.Port = _emailSettings.Port;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
        client.EnableSsl = true;

        await client.SendMailAsync(emailMessage);
    }
}
