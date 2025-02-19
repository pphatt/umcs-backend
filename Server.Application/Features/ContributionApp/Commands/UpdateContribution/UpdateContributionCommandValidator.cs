using FluentValidation;

namespace Server.Application.Features.ContributionApp.Commands.UpdateContribution;

public class UpdateContributionCommandValidator : AbstractValidator<UpdateContributionCommand>
{
    public UpdateContributionCommandValidator()
    {
    }
}
