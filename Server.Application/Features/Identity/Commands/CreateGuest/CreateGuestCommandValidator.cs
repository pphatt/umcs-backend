using FluentValidation;

namespace Server.Application.Features.Identity.Commands.CreateGuest;

public class CreateGuestCommandValidator : AbstractValidator<CreateGuestCommand>
{
    public CreateGuestCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Username)
            .MaximumLength(256)
            .NotEmpty()
            .NotNull();
    }
}
