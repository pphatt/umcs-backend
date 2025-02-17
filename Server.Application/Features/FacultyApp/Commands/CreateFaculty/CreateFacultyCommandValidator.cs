using FluentValidation;

namespace Server.Application.Features.FacultyApp.Commands.CreateFaculty;

public class CreateFacultyCommandValidator : AbstractValidator<CreateFacultyCommand>
{
    public CreateFacultyCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(256)
            .NotEmpty()
            .NotNull();
    }
}
