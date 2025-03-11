using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.ContributionApp.Queries.GetContributionBySlug;

public class GetContributionBySlugQueryHandler : IRequestHandler<GetContributionBySlugQuery, ErrorOr<ResponseWrapper<ContributionWithCommentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public GetContributionBySlugQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ResponseWrapper<ContributionWithCommentDto>>> Handle(GetContributionBySlugQuery request, CancellationToken cancellationToken)
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

        var comments = await _unitOfWork.ContributionCommentRepository.GetCommentsByContributionId(contribution.Id);

        var result = _mapper.Map<ContributionWithCommentDto>(contribution);

        result.Comments = comments;

        return new ResponseWrapper<ContributionWithCommentDto>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
