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

namespace Server.Application.Features.ContributionApp.Queries.GetPersonalContributionDetailBySlug;

public class GetPersonalContributionDetailBySlugQueryHandler : IRequestHandler<GetPersonalContributionDetailBySlugQuery, ErrorOr<ResponseWrapper<ContributionWithCommentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public GetPersonalContributionDetailBySlugQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ResponseWrapper<ContributionWithCommentDto>>> Handle(GetPersonalContributionDetailBySlugQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var contribution = await _unitOfWork.ContributionRepository.GetContributionBySlug(request.Slug);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
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
