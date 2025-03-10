using FluentValidation;

namespace Server.Application.Features.ContributionTagApp.Commands.UpdateTag;

public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.TagName)
            .NotNull()
            .NotEmpty()
            .MaximumLength(256);
    }
}
