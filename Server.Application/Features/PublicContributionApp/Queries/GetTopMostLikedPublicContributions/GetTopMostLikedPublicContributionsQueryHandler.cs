using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Queries.GetTopMostLikedPublicContributions;

public class GetTopMostLikedPublicContributionsQueryHandler : IRequestHandler<GetTopMostLikedPublicContributionsQuery, ErrorOr<ResponseWrapper<PaginationResult<PublicContributionInListDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public GetTopMostLikedPublicContributionsQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<PublicContributionInListDto>>>> Handle(GetTopMostLikedPublicContributionsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var role = await _userManager.GetRolesAsync(user);

        if (role.Contains(Roles.Student))
        {
            request.AllowedGuest = null;
        }

        if (role.Contains(Roles.Guest))
        {
            request.AllowedGuest = true;
        }

        var result = await _unitOfWork.ContributionPublicRepository.GetTopMostLikedPublicContributionsPagination(
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            academicYearName: request.AcademicYearName,
            facultyName: request.FacultyName,
            allowedGuest: request.AllowedGuest
        );

        foreach (var item in result.Results)
        {
            item.AlreadyLike = await _unitOfWork.LikeRepository.AlreadyLike(item.Id, user.Id);
        }

        return new ResponseWrapper<PaginationResult<PublicContributionInListDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
