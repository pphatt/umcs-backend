using FluentValidation;

namespace Server.Application.Features.Identity.Commands.ForgotPassword;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty()
            .NotNull();
    }
}
