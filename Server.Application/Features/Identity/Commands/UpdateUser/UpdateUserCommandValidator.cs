using FluentValidation;

namespace Server.Application.Features.Identity.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
    }
}
