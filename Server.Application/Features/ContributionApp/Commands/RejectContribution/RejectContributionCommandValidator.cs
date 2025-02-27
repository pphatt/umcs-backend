using FluentValidation;

namespace Server.Application.Features.ContributionApp.Commands.RejectContribution;

public class RejectContributionCommandValidator : AbstractValidator<RejectContributionCommand>
{
    public RejectContributionCommandValidator()
    {
    }
}
