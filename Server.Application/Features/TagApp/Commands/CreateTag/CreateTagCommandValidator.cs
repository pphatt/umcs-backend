using FluentValidation;

namespace Server.Application.Features.TagApp.Commands.CreateTag;

public class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.TagName)
            .NotNull()
            .NotEmpty()
            .MaximumLength(256);
    }
}
