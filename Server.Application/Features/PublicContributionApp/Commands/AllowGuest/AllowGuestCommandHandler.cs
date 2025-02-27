using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.PublicContributionApp.Commands.AllowGuest;

public class AllowGuestCommandHandler : IRequestHandler<AllowGuestCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AllowGuestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(AllowGuestCommand request, CancellationToken cancellationToken)
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

        var faculty = await _unitOfWork.FacultyRepository.GetByIdAsync(request.FacultyId);

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

        contribution.AllowedGuest = true;
        publicContribution.AllowedGuest = true;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Allow guest successfully."
        };
    }
}
