using FluentValidation;

namespace Server.Application.Features.PublicContributionCommentApp.Commands;

public class CreatePublicCommentCommandValidator : AbstractValidator<CreatePublicCommentCommand>
{
    public CreatePublicCommentCommandValidator()
    {
        RuleFor(x => x.Content)
            .MaximumLength(500)
            .WithMessage("Comment maximum characters length is 500.");
    }
}
