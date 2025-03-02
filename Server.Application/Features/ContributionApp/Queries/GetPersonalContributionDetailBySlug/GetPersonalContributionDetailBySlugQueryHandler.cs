using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.ContributionApp.Queries.GetPersonalContributionDetailBySlug;

public class GetPersonalContributionDetailBySlugQueryHandler : IRequestHandler<GetPersonalContributionDetailBySlugQuery, ErrorOr<ResponseWrapper<PublicContributionDetailsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public GetPersonalContributionDetailBySlugQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper<PublicContributionDetailsDto>>> Handle(GetPersonalContributionDetailBySlugQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var contribution = await _unitOfWork.ContributionRepository.GetContributionBySlugAndFaculty(request.Slug, user.FacultyId!.Value);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        if (contribution.PublicDate.HasValue)
        {
            // if the contribution is public, get from public contribution instead because of view, like, ...
            var res = await _unitOfWork.ContributionPublicRepository.GetPublicContributionBySlug(request.Slug);

            return new ResponseWrapper<PublicContributionDetailsDto>
            {
                IsSuccessful = true,
                ResponseData = res
            };
        }

        var result = await _unitOfWork.ContributionRepository.GetPersonalContributionBySlug(request.Slug);

        return new ResponseWrapper<PublicContributionDetailsDto>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
