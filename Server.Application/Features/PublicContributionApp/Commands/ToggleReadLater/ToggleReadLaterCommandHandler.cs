using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Commands.ToggleReadLater;

public class ToggleReadLaterCommandHandler : IRequestHandler<ToggleReadLaterCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public ToggleReadLaterCommandHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(ToggleReadLaterCommand request, CancellationToken cancellationToken)
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

        var alreadySave = await _unitOfWork.ContributionPublicReadLaterRepository.AlreadySave(contribution.Id, user.Id);

        if (alreadySave)
        {
            var save = await _unitOfWork.ContributionPublicReadLaterRepository.GetSpecificSave(contribution.Id, user.Id);

            _unitOfWork.ContributionPublicReadLaterRepository.Remove(save);

            await _unitOfWork.CompleteAsync();

            return new ResponseWrapper
            {
                IsSuccessful = true,
                Message = "Remove contribution from read later successfully."
            };
        }

        _unitOfWork.ContributionPublicReadLaterRepository.Add(new ContributionPublicReadLater
        {
            UserId = user.Id,
            ContributionId = contribution.Id
        });

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Save contribution into read later successfully."
        };
    }
}
