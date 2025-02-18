using FluentValidation;

namespace Server.Application.Features.AcademicYearsApp.Commands.BulkDeleteAcademicYears;

public class BulkDeleteAcademicYearsCommandValidator : AbstractValidator<BulkDeleteAcademicYearsCommand>
{
    public BulkDeleteAcademicYearsCommandValidator()
    {
    }
}
