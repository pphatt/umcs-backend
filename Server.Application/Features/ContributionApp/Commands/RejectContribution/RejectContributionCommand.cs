using ErrorOr;
using MediatR;
using Server.Application.Wrapper;
using Server.Domain.Entity.Content;

namespace Server.Application.Features.ContributionApp.Commands.RejectContribution;

public class RejectContributionCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }
}
