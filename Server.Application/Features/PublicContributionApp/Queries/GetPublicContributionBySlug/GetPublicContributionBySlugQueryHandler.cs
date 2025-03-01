using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Queries.GetPublicContributionBySlug;

public class GetPublicContributionBySlugQueryHandler : IRequestHandler<GetPublicContributionBySlugQuery, ErrorOr<ResponseWrapper<PublicContributionDetailsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public GetPublicContributionBySlugQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper<PublicContributionDetailsDto>>> Handle(GetPublicContributionBySlugQuery request, CancellationToken cancellationToken)
    {
        var publicContributionDto = await _unitOfWork.ContributionPublicRepository.GetPublicContributionBySlug(request.Slug);

        if (publicContributionDto is null)
        {
            return Errors.Contribution.CannotFound;
        }

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var role = await _userManager.GetRolesAsync(user);

        if (role.Contains(Roles.Guest))
        {
            if (!publicContributionDto.AllowedGuest)
            {
                return Errors.Contribution.NotAllowed;
            }
        }

        var publicContribution = await _unitOfWork.ContributionPublicRepository.GetByIdAsync(publicContributionDto.Id);

        publicContribution.Views += 1;
        publicContributionDto.View = publicContribution.Views;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper<PublicContributionDetailsDto>
        {
            IsSuccessful = true,
            ResponseData = publicContributionDto
        };
    }
}
