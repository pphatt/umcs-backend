using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Features.PublicContributionApp.Commands.AllowGuestWithManyContributions;

public class AllowGuestWithManyContributionsCommandHandler : IRequestHandler<AllowGuestWithManyContributionsCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AllowGuestWithManyContributionsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(AllowGuestWithManyContributionsCommand request, CancellationToken cancellationToken)
    {
        var contributionIds = request.ContributionIds;

        var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.FacultyId);

        if (faculty is null)
        {
            return Errors.Faculty.CannotFound;
        }

        if (contributionIds.Count() == 0)
        {
            return Errors.Contribution.CannotFound;
        }

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

            await _unitOfWork.CompleteAsync();
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Allow guest to all contributions successfully."
        };
    }
}
