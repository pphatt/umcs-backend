using FluentValidation;

namespace Server.Application.Features.PublicContributionApp.Commands.AllowGuestWithManyContributions;

public class AllowGuestWithManyContributionsCommandValidator : AbstractValidator<AllowGuestWithManyContributionsCommand>
{
    public AllowGuestWithManyContributionsCommandValidator()
    {
    }
}
