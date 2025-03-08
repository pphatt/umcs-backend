using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Extensions;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Enums;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Queries.GetTopContributors;

public class GetTopContributorsQueryHandler : IRequestHandler<GetTopContributorsQuery, ErrorOr<ResponseWrapper<PaginationResult<ContributorDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public GetTopContributorsQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<ContributorDto>>>> Handle(GetTopContributorsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var role = await _userManager.GetRolesAsync(user);

        if (role.Contains(Roles.Student))
        {
            request.OrderBy = OrderByEnum.Descending.ToStringValue();
        }

        if (role.Contains(Roles.Guest))
        {
            request.OrderBy = OrderByEnum.Descending.ToStringValue();
        }

        var result = await _unitOfWork.ContributionPublicRepository.GetTopContributors(
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            facultyName: request.FacultyName,
            orderBy: request.OrderBy
        );

        return new ResponseWrapper<PaginationResult<ContributorDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
