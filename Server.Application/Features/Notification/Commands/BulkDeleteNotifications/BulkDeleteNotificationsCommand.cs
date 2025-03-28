using ErrorOr;

using MediatR;

using Server.Application.Wrapper;

namespace Server.Application.Features.Notification.Commands.BulkDeleteNotifications;

public class BulkDeleteNotificationsCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public List<Guid> Ids { get; set; }
    public Guid UserId { get; set; }
}
