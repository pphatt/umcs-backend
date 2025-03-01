using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Wrapper;

namespace Server.Application.Features.ContributionActivityLogsApp.Queries.GetContributionActivityLogById;

public class GetContributionActivityLogByIdQuery : IRequest<ErrorOr<ResponseWrapper<ContributionActivityLogDto>>>
{
    public Guid Id { get; set; }
}
