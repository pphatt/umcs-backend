using FluentValidation;

namespace Server.Application.Features.PublicContributionApp.Commands.RevokeAllowGuest;

public class RevokeAllowGuestCommandValidator : AbstractValidator<RevokeAllowGuestCommand>
{
    public RevokeAllowGuestCommandValidator()
    {
    }
}
