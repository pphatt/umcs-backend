using FluentValidation;

namespace Server.Application.Features.AcademicYearsApp.Commands.ActiveAcademicYear;

public class ActiveAcademicYearCommandValidator : AbstractValidator<ActiveAcademicYearCommand>
{
    public ActiveAcademicYearCommandValidator()
    {
    }
}
