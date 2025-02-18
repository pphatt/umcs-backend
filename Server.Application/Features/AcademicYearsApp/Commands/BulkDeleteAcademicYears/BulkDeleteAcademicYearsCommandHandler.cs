using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.AcademicYearsApp.Commands.BulkDeleteAcademicYears;

public class BulkDeleteAcademicYearsCommandHandler : IRequestHandler<BulkDeleteAcademicYearsCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BulkDeleteAcademicYearsCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(BulkDeleteAcademicYearsCommand request, CancellationToken cancellationToken)
    {
        var academicYearIds = request.AcademicIds;
        var successfullyDeletedItems = new List<Guid>();

        foreach (var id in academicYearIds)
        {
            var academicYear = await _unitOfWork.AcademicYearRepository.GetByIdAsync(id);

            if (academicYear is null)
            {
                return Errors.AcademicYears.CannotFound;
            }

            var hasContributions = await _unitOfWork.AcademicYearRepository.HasContributionsAsync(id);

            if (hasContributions)
            {
                return Errors.AcademicYears.HasContributions;
            }

            academicYear.DateDeleted = _dateTimeProvider.UtcNow;

            successfullyDeletedItems.Add(id);
        }

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Messages = new List<string>
            {
                $"Successfully deleted {successfullyDeletedItems.Count} academic years.",
                "Each item is available for recovery."
            }
        };
    }
}
