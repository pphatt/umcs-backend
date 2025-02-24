using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ErrorOr<ResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IDataProtector _dataProtector;
    private readonly IEmailService _emailService;

    public ResetPasswordCommandHandler(UserManager<AppUser> userManager, IDataProtectionProvider dataProtectionProvider, IEmailService emailService)
    {
        _userManager = userManager;
        _dataProtector = dataProtectionProvider.CreateProtector("DataProtectorTokenProvider");
        _emailService = emailService;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // Token is considered of:
        // 1. DateTimeOffset.UtcNow
        // 2. UserId
        // 3. Purpose string
        // 4. UserSecurityStamp, if supported
        // More info: https://stackoverflow.com/a/59850344

        var token = Uri.UnescapeDataString(request.Token);
        var resetTokenArray = Convert.FromBase64String(token);
        var unprotectedResetTokenData = _dataProtector.Unprotect(resetTokenArray);

        using (var ms = new MemoryStream(unprotectedResetTokenData))
        {
            using (var reader = new BinaryReader(ms))
            {
                var timeTokenCreated = new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);

                var userId = reader.ReadString();

                var user = await _userManager.FindByIdAsync(userId);

                if (user is null)
                {
                    return Errors.User.CannotFound;
                }

                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, request.Password);

                if (!resetPasswordResult.Succeeded)
                {
                    return Errors.User.FailResetPassword;
                }

                var emailMessage = new MailRequest
                {
                    ToEmail = user.Email ?? string.Empty,
                    Subject = "RESET PASSWORD SUCCESSFULLY",
                    Body = @"Hi,<br><br>
                            Your account password has been reset successfully.<br><br>
                            If you did not request this change, please contact our support team immediately.<br><br>
                            Best regards,<br>
                            The Account Security Team"
                };

                await _emailService.SendEmailAsync(emailMessage);

                return new ResponseWrapper
                {
                    IsSuccessful = true,
                    Message = "Reset password successfully."
                };
            }
        }
    }
}
