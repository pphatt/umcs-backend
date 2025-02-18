using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.AcademicYearsApp.Commands.UpdateAcademicYear;

public class UpdateAcademicYearCommandHandler : IRequestHandler<UpdateAcademicYearCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAcademicYearCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(UpdateAcademicYearCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await _unitOfWork.AcademicYearRepository.GetByIdAsync(request.Id);

        if (academicYear is null)
        {
            return Errors.AcademicYears.CannotFound;
        }

        var nameExists = await _unitOfWork.AcademicYearRepository.GetAcademicYearByNameAsync(request.AcademicYearName);

        if (nameExists is not null && nameExists.Id != academicYear.Id)
        {
            return Errors.AcademicYears.DuplicateName;
        }

        academicYear.Name = request.AcademicYearName;
        academicYear.StartClosureDate = request.StartClosureDate;
        academicYear.EndClosureDate = request.EndClosureDate;
        academicYear.FinalClosureDate = request.FinalClosureDate;
        academicYear.IsActive = request.IsActive;
        academicYear.DateUpdated = DateTime.UtcNow;

        _unitOfWork.AcademicYearRepository.Update(academicYear);

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Update academic year successfully."
        };
    }
}
