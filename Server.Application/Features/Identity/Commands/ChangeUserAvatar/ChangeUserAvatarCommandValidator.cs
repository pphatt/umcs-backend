using FluentValidation;

namespace Server.Application.Features.Identity.Commands.ChangeUserAvatar;

public class ChangeUserAvatarCommandValidator : AbstractValidator<ChangeUserAvatarCommand>
{
    public ChangeUserAvatarCommandValidator()
    {
    }
}
