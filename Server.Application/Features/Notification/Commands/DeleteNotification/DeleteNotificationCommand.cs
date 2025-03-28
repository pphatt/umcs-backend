using ErrorOr;

using MediatR;

using Server.Application.Wrapper;

namespace Server.Application.Features.Notification.Commands.DeleteNotification;

public class DeleteNotificationCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
