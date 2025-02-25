using FluentValidation;

namespace Server.Application.Features.ContributionApp.Commands.ApproveContribution;

public class ApproveContributionCommandValidator : AbstractValidator<ApproveContributionCommand>
{
    public ApproveContributionCommandValidator()
    {
    }
}
