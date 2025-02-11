using FluentValidation;

namespace Server.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Username)
            .MaximumLength(256)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.FirstName)
            .MaximumLength(256);

        RuleFor(x => x.LastName)
            .MaximumLength(256);

        RuleFor(x => x.Dob)
            .Must(dob => dob == null || AtLeast18YearsOld(dob.Value))
            .WithMessage("User must be at least 18 years old.");
    }

    private bool AtLeast18YearsOld(DateTime dob)
    {
        return dob <= DateTime.Today.AddYears(-18);
    }
}
