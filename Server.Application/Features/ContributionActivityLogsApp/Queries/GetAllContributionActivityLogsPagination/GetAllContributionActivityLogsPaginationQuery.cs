using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.ContributionActivityLogsApp.Queries.GetAllContributionActivityLogsPagination;

public class GetAllContributionActivityLogsPaginationQuery : IRequest<ErrorOr<ResponseWrapper<PaginationResult<ContributionActivityLogsDto>>>>
{
    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? FacultyName { get; set; }

    public string? AcademicYearName { get; set; }
}
