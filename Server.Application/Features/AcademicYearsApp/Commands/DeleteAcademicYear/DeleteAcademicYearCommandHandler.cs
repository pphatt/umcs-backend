using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.AcademicYearsApp.Commands.DeleteAcademicYear;

public class DeleteAcademicYearCommandHandler : IRequestHandler<DeleteAcademicYearCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeleteAcademicYearCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(DeleteAcademicYearCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await _unitOfWork.AcademicYearRepository.GetByIdAsync(request.Id);

        if (academicYear is null)
        {
            return Errors.AcademicYears.CannotFound;
        }

        var hasContributions = await _unitOfWork.AcademicYearRepository.HasContributionsAsync(request.Id);

        if (hasContributions)
        {
            return Errors.AcademicYears.HasContributions;
        }

        academicYear.DateDeleted = _dateTimeProvider.UtcNow;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Delete academic year successfully."
        };
    }
}
