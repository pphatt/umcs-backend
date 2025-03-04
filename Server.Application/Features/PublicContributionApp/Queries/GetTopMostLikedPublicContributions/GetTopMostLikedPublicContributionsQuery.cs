using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.PublicContributionApp.Queries.GetTopMostLikedPublicContributions;

public class GetTopMostLikedPublicContributionsQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<PublicContributionInListDto>>>>
{
    public Guid UserId { get; set; } = default!;

    public string? AcademicYearName { get; set; }

    public string? FacultyName { get; set; }

    public bool? AllowedGuest { get; set; }
}
