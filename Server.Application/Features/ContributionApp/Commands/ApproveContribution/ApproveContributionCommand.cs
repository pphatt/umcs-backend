using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.ContributionApp.Commands.ApproveContribution;

public class ApproveContributionCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }

    public Guid CoordinatorId { get; set; }
}
