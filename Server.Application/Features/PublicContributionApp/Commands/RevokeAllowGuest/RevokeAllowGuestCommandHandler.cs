using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Enums;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Commands.RevokeAllowGuest;

public class RevokeAllowGuestCommandHandler : IRequestHandler<RevokeAllowGuestCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public RevokeAllowGuestCommandHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(RevokeAllowGuestCommand request, CancellationToken cancellationToken)
    {
        var contribution = await _unitOfWork.ContributionRepository.GetByIdAsync(request.ContributionId);

        if (contribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        var publicContribution = await _unitOfWork.ContributionPublicRepository.GetByIdAsync(request.ContributionId);

        if (publicContribution is null)
        {
            return Errors.Contribution.CannotFound;
        }

        if (!contribution.AllowedGuest || !publicContribution.AllowedGuest)
        {
            return Errors.Contribution.NotAllowYet;
        }

        // coordinator, admin, ...
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.UserFacultyId);

        if (faculty is null)
        {
            return Errors.Faculty.CannotFound;
        }

        if (faculty.Id != contribution.FacultyId)
        {
            return Errors.Contribution.NotBelongToFaculty;
        }

        if (!contribution.PublicDate.HasValue)
        {
            return Errors.Contribution.NotPublicYet;
        }

        contribution.AllowedGuest = false;
        publicContribution.AllowedGuest = false;

        await _unitOfWork.CompleteAsync();

        _unitOfWork.ContributionActivityLogRepository.Add(new ContributionActivityLog
        {
            ContributionId = contribution.Id,
            ContributionTitle = contribution.Title,
            UserId = user.Id,
            Username = user.UserName,
            FromStatus = ContributionStatus.Approve,
            ToStatus = ContributionStatus.Approve,
            Description = $"{user.UserName} revoke allow guest to view the public contribution.",
        });

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Revoke allow guest successfully."
        };
    }
}
