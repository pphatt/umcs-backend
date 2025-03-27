using ErrorOr;

using MediatR;

using Server.Application.Wrapper;

namespace Server.Application.Features.Notification.Commands.MarkAllNotificationsAsRed;

public class MarkAllNotificationsAsRedCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid UserId { get; set; }
}
