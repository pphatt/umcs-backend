using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.PublicContributionApp.Commands.ToggleLikeContribution;

public class ToggleLikeContributionCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid ContributionId { get; set; }

    public Guid UserId { get; set; }
}
