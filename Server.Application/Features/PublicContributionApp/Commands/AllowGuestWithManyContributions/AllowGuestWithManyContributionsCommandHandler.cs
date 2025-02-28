using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Enums;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PublicContributionApp.Commands.AllowGuestWithManyContributions;

public class AllowGuestWithManyContributionsCommandHandler : IRequestHandler<AllowGuestWithManyContributionsCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public AllowGuestWithManyContributionsCommandHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(AllowGuestWithManyContributionsCommand request, CancellationToken cancellationToken)
    {
        var contributionIds = request.ContributionIds;

        var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.UserFacultyId);

        if (faculty is null)
        {
            return Errors.Faculty.CannotFound;
        }

        if (contributionIds.Count() == 0)
        {
            return Errors.Contribution.CannotFound;
        }

        // coordinator, admin, ...
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var publicContributionList = new List<ContributionPublic>();

        foreach (var id in contributionIds)
        {
            var contribution = await _unitOfWork.ContributionRepository.GetByIdAsync(id);

            if (contribution is null)
            {
                return Errors.Contribution.CannotFound;
            }

            if (contribution.FacultyId != faculty.Id)
            {
                return Errors.Contribution.NotBelongToFaculty;
            }

            if (!contribution.PublicDate.HasValue)
            {
                return Errors.Contribution.NotPublicYet;
            }

            // cannot revoke many while allow many, there will be an api only for revoke many.
            if (contribution.AllowedGuest)
            {
                return Errors.Contribution.AlreadyAllowGuest;
            }

            var publicContribution = await _unitOfWork.ContributionPublicRepository.GetByIdAsync(id);

            if (publicContribution is null)
            {
                return Errors.Contribution.CannotFound;
            }

            if (publicContribution.AllowedGuest)
            {
                return Errors.Contribution.AlreadyAllowGuest;
            }

            contribution.AllowedGuest = true;
            publicContribution.AllowedGuest = true;

            publicContributionList.Add(publicContribution);
        }

        foreach (var contribution in publicContributionList)
        {
            _unitOfWork.ContributionActivityLogRepository.Add(new ContributionActivityLog
            {
                ContributionId = contribution.Id,
                ContributionTitle = contribution.Title,
                UserId = user.Id,
                Username = user.UserName,
                FromStatus = ContributionStatus.Approve,
                ToStatus = ContributionStatus.Approve,
                Description = $"{user.UserName} allow guest to view the public contribution.",
            });
        }

        // reason the move the save outside is that if one of the contribution throw, it will cancel the transaction.
        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Allow guest to all contributions successfully."
        };
    }
}
