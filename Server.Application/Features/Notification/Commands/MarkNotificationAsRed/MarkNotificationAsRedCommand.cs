using ErrorOr;

using MediatR;

using Server.Application.Wrapper;

namespace Server.Application.Features.Notification.Commands.MarkNotificationAsRed;

public class MarkNotificationAsRedCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
}
