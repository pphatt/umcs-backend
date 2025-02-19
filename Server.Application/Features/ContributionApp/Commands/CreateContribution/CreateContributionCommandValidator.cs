using FluentValidation;

namespace Server.Application.Features.ContributionApp.Commands.CreateContribution;

public class CreateContributionCommandValidator : AbstractValidator<CreateContributionCommand>
{
    public CreateContributionCommandValidator()
    {
    }
}
