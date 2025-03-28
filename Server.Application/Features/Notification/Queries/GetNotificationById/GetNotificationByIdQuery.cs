using ErrorOr;

using MediatR;

using Server.Application.Common.Dtos.Content.Notification;
using Server.Application.Wrapper;

namespace Server.Application.Features.Notification.Queries.GetNotificationById;

public class GetNotificationByIdQuery : IRequest<ErrorOr<ResponseWrapper<NotificationDto>>>
{
    public Guid Id { get; set; }
}
