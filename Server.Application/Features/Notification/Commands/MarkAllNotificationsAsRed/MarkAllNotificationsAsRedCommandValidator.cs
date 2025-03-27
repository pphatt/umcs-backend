using FluentValidation;

namespace Server.Application.Features.Notification.Commands.MarkAllNotificationsAsRed;

public class MarkAllNotificationsAsRedCommandValidator : AbstractValidator<MarkAllNotificationsAsRedCommand>
{
    public MarkAllNotificationsAsRedCommandValidator()
    {
    }
}
