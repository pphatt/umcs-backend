using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Commands.RatePublicContribution;

public class RatePublicContributionCommandHandler : IRequestHandler<RatePublicContributionCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public RatePublicContributionCommandHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(RatePublicContributionCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var contribution = await _unitOfWork.ContributionPublicRepository.GetByIdAsync(request.ContributionId);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains(Roles.Guest) && !contribution.AllowedGuest)
        {
            return Errors.Contribution.NotAllowYet;
        }

        var alreadyRate = await _unitOfWork.ContributionPublicRatingRepository.AlreadyRate(contributionId: contribution.Id, userId: user.Id);
        var averageRating = 0.0;

        if (alreadyRate)
        {
            var rating = await _unitOfWork.ContributionPublicRatingRepository.GetSpecificRating(contribution.Id, user.Id);
            rating.Rating = request.Rating;

            averageRating = await _unitOfWork.ContributionPublicRatingRepository.GetContributionAverageRating(contribution.Id);
            contribution.AverageRating = averageRating;

            await _unitOfWork.CompleteAsync();

            return new ResponseWrapper
            {
                IsSuccessful = true,
                Message = "Rate contribution successfully."
            };
        }

        var rate = new ContributionPublicRating
        {
            ContributionId = contribution.Id,
            UserId = user.Id,
            Rating = request.Rating,
        };

        _unitOfWork.ContributionPublicRatingRepository.Add(rate);

        await _unitOfWork.CompleteAsync();

        averageRating = await _unitOfWork.ContributionPublicRatingRepository.GetContributionAverageRating(contribution.Id);
        contribution.AverageRating = averageRating;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Rate contribution successfully."
        };
    }
}
