using Server.Contracts.Common.Email;

namespace Server.Application.Common.Interfaces.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(MailRequest email);
}
