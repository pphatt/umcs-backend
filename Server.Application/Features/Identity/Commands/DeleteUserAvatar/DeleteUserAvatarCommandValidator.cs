using FluentValidation;

namespace Server.Application.Features.Identity.Commands.DeleteUserAvatar;

public class DeleteUserAvatarCommandValidator : AbstractValidator<DeleteUserAvatarCommand>
{
    public DeleteUserAvatarCommandValidator()
    {
    }
}
