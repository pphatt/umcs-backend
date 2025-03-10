using FluentValidation;

namespace Server.Application.Features.ContributionTagApp.Commands.DeleteTag;

public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
    }
}
