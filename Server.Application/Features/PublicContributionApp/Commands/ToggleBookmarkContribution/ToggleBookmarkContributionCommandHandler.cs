using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Commands.ToggleBookmarkContribution;

public class ToggleBookmarkContributionCommandHandler : IRequestHandler<ToggleBookmarkContributionCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public ToggleBookmarkContributionCommandHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(ToggleBookmarkContributionCommand request, CancellationToken cancellationToken)
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
            return Errors.Contribution.CannotFound;
        }

        var alreadyBookmark = await _unitOfWork.ContributionPublicBookmarkRepository.AlreadyBookmark(contribution.Id, user.Id);

        if (alreadyBookmark)
        {
            var bookmark = await _unitOfWork.ContributionPublicBookmarkRepository.GetSpecificBookmark(contribution.Id, user.Id);

            _unitOfWork.ContributionPublicBookmarkRepository.Remove(bookmark);

            await _unitOfWork.CompleteAsync();

            return new ResponseWrapper
            {
                IsSuccessful = true,
                Message = "Remove contribution from bookmark successfully."
            };
        }

        _unitOfWork.ContributionPublicBookmarkRepository.Add(new ContributionPublicBookmark
        {
            ContributionId = contribution.Id,
            UserId = user.Id
        });

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Bookmark contribution successfully."
        };
    }
}
