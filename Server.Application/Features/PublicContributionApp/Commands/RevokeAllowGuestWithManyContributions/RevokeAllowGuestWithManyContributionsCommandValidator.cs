using FluentValidation;

namespace Server.Application.Features.PublicContributionApp.Commands.RevokeAllowGuestWithManyContributions;

public class RevokeAllowGuestWithManyContributionsCommandValidator : AbstractValidator<RevokeAllowGuestWithManyContributionsCommand>
{
    public RevokeAllowGuestWithManyContributionsCommandValidator()
    {
    }
}
