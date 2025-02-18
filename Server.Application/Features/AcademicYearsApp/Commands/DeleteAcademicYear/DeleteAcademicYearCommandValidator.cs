using FluentValidation;

namespace Server.Application.Features.AcademicYearsApp.Commands.DeleteAcademicYear;

public class DeleteAcademicYearCommandValidator : AbstractValidator<DeleteAcademicYearCommand>
{
    public DeleteAcademicYearCommandValidator()
    {
    }
}
