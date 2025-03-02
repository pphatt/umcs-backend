using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.Like;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.PublicContributionApp.Queries.GetListUserLiked;

public class GetAllUsersLikedContributionPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<UserLikeInListDto>>>>
{
    public Guid ContributionId { get; set; }
}
