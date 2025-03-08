using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.PublicContributionApp.Commands.RatePublicContribution;

public class RatePublicContributionCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid UserId { get; set; }

    public Guid ContributionId { get; set; }

    public double Rating { get; set; }
}
