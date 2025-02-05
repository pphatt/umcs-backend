using FluentValidation;

namespace Server.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotNull()
            .NotEmpty()
            .WithMessage("Refresh token cannot be null or empty.");
    }
}
