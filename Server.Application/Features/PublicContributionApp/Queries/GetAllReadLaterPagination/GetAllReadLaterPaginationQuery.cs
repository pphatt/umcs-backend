using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.PublicContributionApp.Queries.GetAllReadLaterPagination;

public class GetAllReadLaterPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<PublicContributionInListDto>>>>
{
    public Guid UserId { get; set; }

    public string FacultyName { get; set; }

    public string AcademicYearName { get; set; }

    public string OrderBy { get; set; }
}
