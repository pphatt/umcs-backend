using FluentValidation;

namespace Server.Application.Features.PublicContributionApp.Commands.ToggleLikeContribution;

public class ToggleLikeContributionCommandValidator : AbstractValidator<ToggleLikeContributionCommand>
{
    public ToggleLikeContributionCommandValidator()
    {
    }
}
