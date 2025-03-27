using FluentValidation;

namespace Server.Application.Features.Notification.Commands.UnreadAllNotifications;

public class UnreadAllNotificationsCommandValidator : AbstractValidator<UnreadAllNotificationsCommand>
{
    public UnreadAllNotificationsCommandValidator()
    {
    }
}
