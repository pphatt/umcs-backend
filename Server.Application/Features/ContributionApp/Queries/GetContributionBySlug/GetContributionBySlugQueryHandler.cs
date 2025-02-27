using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;
using System.Runtime.InteropServices;

namespace Server.Application.Features.ContributionApp.Queries.GetContributionBySlug;

public class GetContributionBySlugQueryHandler : IRequestHandler<GetContributionBySlugQuery, ErrorOr<ResponseWrapper<ContributionDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public GetContributionBySlugQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper<ContributionDto>>> Handle(GetContributionBySlugQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FacultyName))
        {
            return Errors.Faculty.MissingName;
        }

        var faculty = await _unitOfWork.FacultyRepository.GetFacultyByNameAsync(request.FacultyName);

        if (faculty is null)
        {
            return Errors.Faculty.CannotFound;
        }

        var contribution = await _unitOfWork.ContributionRepository.GetContributionBySlugAndFaculty(request.Slug, faculty.Id);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        // can be faculty coordinator or owner's contribution.
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains(Roles.Student) && user.UserName != contribution.Username)
        {
            return Errors.User.NotOwnedContribution;
        }

        if (roles.Contains(Roles.Guest))
        {
            return Errors.Contribution.NotPublicYet;
        }

        return new ResponseWrapper<ContributionDto>
        {
            IsSuccessful = true,
            ResponseData = contribution
        };
    }
}
