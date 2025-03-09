using FluentValidation;

namespace Server.Application.Features.Identity.Commands.EditUserProfile;

public class EditUserProfileCommandValidator : AbstractValidator<EditUserProfileCommand>
{
    public EditUserProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(255)
            .NotEmpty()
            .WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .MaximumLength(255)
            .NotEmpty()
            .WithMessage("Last name is required");
    }
}
