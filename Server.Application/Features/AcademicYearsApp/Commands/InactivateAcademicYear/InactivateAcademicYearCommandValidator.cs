using FluentValidation;

namespace Server.Application.Features.AcademicYearsApp.Commands.InactivateAcademicYear;

public class InactivateAcademicYearCommandValidator : AbstractValidator<InactivateAcademicYearCommand>
{
    public InactivateAcademicYearCommandValidator()
    {
    }
}
