using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ErrorOr<ResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public ForgotPasswordCommandHandler(UserManager<AppUser> userManager, IEmailService emailService, IConfiguration configuration)
    {
        _userManager = userManager;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordBaseUrl = _configuration["ApplicationSettings:ResetPasswordBaseUrl"];
        var resetPasswordUrl = $"{resetPasswordBaseUrl}?token={Uri.EscapeDataString(token)}";

        var emailMessage = new MailRequest
        {
            ToEmail = user.Email ?? string.Empty,
            Subject = "Your password reset request",
            Body = $@"
                    <!DOCTYPE html>
                    <html lang=""en"">
                    <head>
                        <meta charset=""UTF-8"">
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        <title>Reset Your Password</title>
                    </head>
                    <body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333333"">
                        <div style=""background-color: #ffffff"">
                            Hi {user.Email},<br>
                            <br>
                            We received your request to reset the password for your account.<br>
                            <br>
                            To reset your password, please click on the following link:<br>
                            <br>
                            <a href=""{resetPasswordUrl}"" style=""background-color: #0067B8; color: #ffffff; padding: 10px 20px; text-decoration: none; border-radius: 4px; display: inline-block;"">Reset Your Password</a><br>
                            <br>
                            If you didn't request this password reset, you can safely ignore this email. Your password won't be changed unless you click the link above.<br>
                            <br>
                            For your security, this link will expire in 30 minutes. Only click this link on an official website. Don't share it with anyone. We'll never ask you to share it outside an official platform.<br>
                            <br>
                            Thanks,<br>
                            The Account Security Team<br>
                        </div>
                    </body>
                    </html>"
        };

        await _emailService.SendEmailAsync(emailMessage);

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = $"Sent email to user '{user.UserName}' successfully. Please check email"
        };
    }
}
