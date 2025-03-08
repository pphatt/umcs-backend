using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Extensions;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Enums;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Queries.GetTopMostViewedPublicContributions;

public class GetTopMostViewedPublicContributionsQueryHandler : IRequestHandler<GetTopMostViewedPublicContributionsQuery, ErrorOr<ResponseWrapper<PaginationResult<PublicContributionInListDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public GetTopMostViewedPublicContributionsQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<PublicContributionInListDto>>>> Handle(GetTopMostViewedPublicContributionsQuery request, CancellationToken cancellationToken)
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
            request.SortBy = ContributionSortBy.View.ToStringValue();
            request.OrderBy = OrderByEnum.Descending.ToStringValue();
        }

        if (role.Contains(Roles.Guest))
        {
            request.AllowedGuest = true;
            request.SortBy = ContributionSortBy.View.ToStringValue();
            request.OrderBy = OrderByEnum.Descending.ToStringValue();
        }

        var result = await _unitOfWork.ContributionPublicRepository.GetAllPublicContributionsPagination(
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            userId: request.UserId,
            academicYearName: request.AcademicYearName,
            facultyName: request.FacultyName,
            allowedGuest: request.AllowedGuest,
            sortBy: request.SortBy,
            orderBy: request.OrderBy
        );

        return new ResponseWrapper<PaginationResult<PublicContributionInListDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
