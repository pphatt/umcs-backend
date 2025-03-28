using FluentValidation;

namespace Server.Application.Features.Notification.Commands.BulkDeleteNotifications;

public class BulkDeleteNotificationsCommandValidator : AbstractValidator<BulkDeleteNotificationsCommand>
{
    public BulkDeleteNotificationsCommandValidator()
    {
    }
}
