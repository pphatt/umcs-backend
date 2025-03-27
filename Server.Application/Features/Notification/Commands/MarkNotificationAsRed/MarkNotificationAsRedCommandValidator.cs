using FluentValidation;

using Server.Contracts.Notifications.MarkNotificationAsRed;

namespace Server.Application.Features.Notification.Commands.MarkNotificationAsRed;

public class MarkNotificationAsRedCommandValidator : AbstractValidator<MarkNotificationAsRedRequest>
{
    public MarkNotificationAsRedCommandValidator()
    {
    }
}
