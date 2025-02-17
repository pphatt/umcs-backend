using FluentValidation;

namespace Server.Application.Features.FacultyApp.Commands.UpdateFaculty;

public class UpdateFacultyCommandValidator : AbstractValidator<UpdateFacultyCommand>
{
    public UpdateFacultyCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(256)
            .NotEmpty()
            .NotNull();
    }
}
