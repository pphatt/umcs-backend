using FluentValidation;

namespace Server.Application.Features.PublicContributionApp.Commands.ToggleReadLater;

public class ToggleReadLaterCommandValidator : AbstractValidator<ToggleReadLaterCommand>
{
    public ToggleReadLaterCommandValidator()
    {
    }
}
