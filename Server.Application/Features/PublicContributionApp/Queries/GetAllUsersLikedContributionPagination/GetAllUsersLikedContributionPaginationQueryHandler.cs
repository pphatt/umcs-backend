using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Like;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Features.PublicContributionApp.Queries.GetListUserLiked;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.PublicContributionApp.Queries.GetAllUsersLikedContributionPagination;

public class GetAllUsersLikedContributionPaginationQueryHandler : IRequestHandler<GetAllUsersLikedContributionPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<UserLikeInListDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersLikedContributionPaginationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<UserLikeInListDto>>>> Handle(GetAllUsersLikedContributionPaginationQuery request, CancellationToken cancellationToken)
    {
        var contribution = await _unitOfWork.ContributionRepository.GetByIdAsync(request.ContributionId);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        var result = await _unitOfWork.ContributionPublicRepository.GetAllUsersLikedContributionPagination(
            contributionId: request.ContributionId,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize
        );

        return new ResponseWrapper<PaginationResult<UserLikeInListDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
