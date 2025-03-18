using FluentValidation;

namespace Server.Application.Features.Identity.Commands.ValidateForgotPasswordToken;

public class ValidateForgotPasswordTokenCommandValidator : AbstractValidator<ValidateForgotPasswordTokenCommand>
{
    public ValidateForgotPasswordTokenCommandValidator()
    {
    }
}
