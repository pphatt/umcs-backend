using FluentValidation;

namespace Server.Application.Features.PublicContributionApp.Commands.RatePublicContribution;

public class RatePublicContributionCommandValidator : AbstractValidator<RatePublicContributionCommand>
{
    public RatePublicContributionCommandValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(0, 10)
            .WithMessage("Rating must be between 0 and 10");
    }
}
