using FluentValidation;

namespace Server.Application.Features.Notification.Commands.UnreadNotification;

public class UnreadNotificationCommandValidator : AbstractValidator<UnreadNotificationCommand>
{
    public UnreadNotificationCommandValidator()
    {
    }
}
