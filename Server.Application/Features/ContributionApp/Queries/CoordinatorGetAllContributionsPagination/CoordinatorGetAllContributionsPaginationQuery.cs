using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.ContributionApp.Queries.CoordinatorGetAllContributionsPagination;

public class CoordinatorGetAllContributionsPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<ContributionInListDto>>>>
{
    public string? AcademicYear { get; set; }

    public string Faculty { get; set; } = default!;

    public string? Status { get; set; }
}
