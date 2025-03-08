using FluentValidation;

namespace Server.Application.Features.PublicContributionApp.Commands.ToggleBookmarkContribution;

public class ToggleBookmarkContributionCommandValidator : AbstractValidator<ToggleBookmarkContributionCommand>
{
    public ToggleBookmarkContributionCommandValidator()
    {
    }
}
