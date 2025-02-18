using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.AcademicYearsApp.Commands.InactivateAcademicYear;

public class InactivateAcademicYearCommandHandler : IRequestHandler<InactivateAcademicYearCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public InactivateAcademicYearCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(InactivateAcademicYearCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await _unitOfWork.AcademicYearRepository.GetByIdAsync(request.Id);

        if (academicYear is null)
        {
            return Errors.AcademicYears.CannotFound;
        }

        academicYear.IsActive = false;
        academicYear.DateUpdated = _dateTimeProvider.UtcNow;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Inactivate academic year successfully."
        };
    }
}
