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

namespace Server.Application.Features.PublicContributionApp.Queries.GetAllPublicContributionsPagination;

public class GetAllPublicContributionsPaginationQueryHandler : IRequestHandler<GetAllPublicContributionsPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<PublicContributionInListDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public GetAllPublicContributionsPaginationQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<PublicContributionInListDto>>>> Handle(GetAllPublicContributionsPaginationQuery request, CancellationToken cancellationToken)
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

        var result = await _unitOfWork.ContributionPublicRepository.GetAllPublicContributionsPagination(
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            academicYearName: request.AcademicYearName,
            facultyName: request.FacultyName,
            allowedGuest: request.AllowedGuest
        );

        return new ResponseWrapper<PaginationResult<PublicContributionInListDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
