using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.AcademicYearsApp.Commands.ActivateAcademicYear;

public class ActivateAcademicYearCommandHandler : IRequestHandler<ActivateAcademicYearCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ActivateAcademicYearCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTImeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTImeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(ActivateAcademicYearCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await _unitOfWork.AcademicYearRepository.GetByIdAsync(request.Id);

        if (academicYear is null)
        {
            return Errors.AcademicYears.CannotFound;
        }

        academicYear.IsActive = true;
        academicYear.DateUpdated = _dateTimeProvider.UtcNow;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Active academic year successfully."
        };
    }
}
