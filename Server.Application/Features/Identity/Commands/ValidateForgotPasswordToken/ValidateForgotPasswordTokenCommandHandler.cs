using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Commands.ValidateForgotPasswordToken;

public class ValidateForgotPasswordTokenCommandHandler : IRequestHandler<ValidateForgotPasswordTokenCommand, ErrorOr<ResponseWrapper<bool>>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IDataProtector _dataProtector;

    public ValidateForgotPasswordTokenCommandHandler(UserManager<AppUser> userManager, IDataProtectionProvider dataProtectionProvider)
    {
        _userManager = userManager;
        _dataProtector = dataProtectionProvider.CreateProtector("DataProtectorTokenProvider");
    }

    public async Task<ErrorOr<ResponseWrapper<bool>>> Handle(ValidateForgotPasswordTokenCommand request, CancellationToken cancellationToken)
    {
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

                var result = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);

                return new ResponseWrapper<bool>
                {
                    IsSuccessful = true,
                    Message = "Validate forgot password token successfully.",
                    ResponseData = result,
                };
            }
        }
    }
}
