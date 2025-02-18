using FluentValidation;
using Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculty;

namespace Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculties;

public class BulkDeleteFacultiesCommandValidator : AbstractValidator<BulkDeleteFacultiesCommand>
{
    public BulkDeleteFacultiesCommandValidator()
    {
    }
}
