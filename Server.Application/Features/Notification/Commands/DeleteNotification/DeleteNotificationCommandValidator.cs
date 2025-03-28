using FluentValidation;

namespace Server.Application.Features.Notification.Commands.DeleteNotification;

public class DeleteNotificationCommandValidator : AbstractValidator<DeleteNotificationCommand>
{
    public DeleteNotificationCommandValidator()
    {
    }
}
