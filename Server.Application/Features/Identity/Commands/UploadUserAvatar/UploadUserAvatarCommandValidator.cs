using FluentValidation;

namespace Server.Application.Features.Identity.Commands.UploadUserAvatar;

public class UploadUserAvatarCommandValidator : AbstractValidator<UploadUserAvatarCommand>
{
    public UploadUserAvatarCommandValidator()
    {
    }
}
