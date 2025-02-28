using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Wrapper;

namespace Server.Application.Features.ContributionActivityLogsApp.Queries.GetContributionActivityLogsByContributionId;

public class GetContributionActivityLogsByContributionIdQuery : IRequest<ErrorOr<ResponseWrapper<List<ContributionActivityLogDto>>>>
{
    public Guid Id { get; set; }
}
