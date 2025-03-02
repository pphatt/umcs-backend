using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Commands.ToggleLikeContribution;

public class ToggleLikeContributionCommandHandler : IRequestHandler<ToggleLikeContributionCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public ToggleLikeContributionCommandHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(ToggleLikeContributionCommand request, CancellationToken cancellationToken)
    {
        var contribution = await _unitOfWork.ContributionPublicRepository.GetByIdAsync(request.ContributionId);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var alreadyLike = await _unitOfWork.LikeRepository.AlreadyLike(contribution.Id, user.Id);

        if (alreadyLike)
        {
            var like = await _unitOfWork.LikeRepository.GetSpecificLike(contribution.Id, user.Id);

            _unitOfWork.LikeRepository.Remove(like);
            contribution.LikeQuantity--;

            await _unitOfWork.CompleteAsync();

            return new ResponseWrapper
            {
                IsSuccessful = true,
                Message = "Unlike contribution successfully."
            };
        }

        _unitOfWork.LikeRepository.Add(new Like { ContributionId = contribution.Id, UserId = user.Id });
        contribution.LikeQuantity++;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Like contribution successfully."
        };
    }
}
