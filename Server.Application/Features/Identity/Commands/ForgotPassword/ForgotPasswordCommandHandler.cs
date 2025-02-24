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
            Subject = "RESET PASSWORD",
            Body = $"Please reset your password by clicking the following link: {resetPasswordUrl}"
        };

        await _emailService.SendEmailAsync(emailMessage);

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = $"Sent email to user '{user.UserName}' successfully. Please check email"
        };
    }
}
