using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.ContributionApp.Queries.GetAllUngradedContributionsPagination;

public class GetAllUngradedContributionsPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<UngradedContributionDto>>>>
{
    public string? AcademicYear { get; set; }

    public string? Faculty { get; set; }

    public string? OrderBy { get; set; }
}
