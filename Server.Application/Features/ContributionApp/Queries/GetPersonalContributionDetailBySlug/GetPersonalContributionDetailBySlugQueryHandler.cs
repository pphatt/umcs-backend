using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.ContributionApp.Queries.GetPersonalContributionDetailBySlug;

public class GetPersonalContributionDetailBySlugQueryHandler : IRequestHandler<GetPersonalContributionDetailBySlugQuery, ErrorOr<ResponseWrapper<ContributionDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public GetPersonalContributionDetailBySlugQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper<ContributionDto>>> Handle(GetPersonalContributionDetailBySlugQuery request, CancellationToken cancellationToken)
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

        var result = await _unitOfWork.ContributionRepository.GetPersonalContributionBySlug(request.Slug, request.UserId);

        return new ResponseWrapper<ContributionDto>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
