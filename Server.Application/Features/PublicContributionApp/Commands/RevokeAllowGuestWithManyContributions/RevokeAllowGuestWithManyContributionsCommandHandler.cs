using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.PublicContributionApp.Commands.RevokeAllowGuestWithManyContributions;

public class RevokeAllowGuestWithManyContributionsCommandHandler : IRequestHandler<RevokeAllowGuestWithManyContributionsCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RevokeAllowGuestWithManyContributionsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(RevokeAllowGuestWithManyContributionsCommand request, CancellationToken cancellationToken)
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

            // cannot revoke many while allow many, there will be an api only for revoke many.
            if (!contribution.AllowedGuest)
            {
                return Errors.Contribution.NotAllowYet;
            }

            var publicContribution = await _unitOfWork.ContributionPublicRepository.GetByIdAsync(id);

            if (publicContribution is null)
            {
                return Errors.Contribution.CannotFound;
            }

            if (!publicContribution.AllowedGuest)
            {
                return Errors.Contribution.NotAllowYet;
            }

            contribution.AllowedGuest = false;
            publicContribution.AllowedGuest = false;

            await _unitOfWork.CompleteAsync();
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Revoke allow guest successfully."
        };
    }
}
