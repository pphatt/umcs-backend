using FluentValidation;

namespace Server.Application.Features.TagApp.Commands.DeleteTag;

public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
    }
}
