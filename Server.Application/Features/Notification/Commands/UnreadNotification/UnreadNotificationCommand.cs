using ErrorOr;

using MediatR;

using Server.Application.Wrapper;

namespace Server.Application.Features.Notification.Commands.UnreadNotification;

public class UnreadNotificationCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
}
