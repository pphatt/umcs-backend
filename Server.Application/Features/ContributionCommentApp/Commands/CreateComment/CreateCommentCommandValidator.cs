using FluentValidation;

namespace Server.Application.Features.ContributionCommentApp.Commands.CreateComment;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.Content)
            .MaximumLength(500)
            .WithMessage("Comment maximum characters length is 500.");
    }
}
