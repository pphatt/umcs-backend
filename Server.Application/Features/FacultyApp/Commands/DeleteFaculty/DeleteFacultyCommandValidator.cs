using FluentValidation;

namespace Server.Application.Features.FacultyApp.Commands.DeleteFaculty;

public class DeleteFacultyCommandValidator : AbstractValidator<DeleteFacultyCommand>
{
    public DeleteFacultyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotNull();
    }
}
