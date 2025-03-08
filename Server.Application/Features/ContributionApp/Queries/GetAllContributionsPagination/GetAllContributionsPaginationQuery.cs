using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.ContributionApp.Queries.GetAllContributionsPagination;

public class GetAllContributionsPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<ContributionInListDto>>>>
{
    public Guid? UserId { get; set; }

    public string FacultyName { get; set; } = default!;

    public string? AcademicYearName { get; set; }

    public bool? AllowedGuest { get; set; }

    public string? Status { get; set; }

    public string? OrderBy { get; set; }
}
