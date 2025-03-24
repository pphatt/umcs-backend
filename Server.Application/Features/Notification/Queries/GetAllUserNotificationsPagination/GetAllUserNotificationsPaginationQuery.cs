using ErrorOr;

using MediatR;

using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.Notification;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.Notification.Queries.GetAllUserNotificationsPagination;

public class GetAllUserNotificationsPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<NotificationDto>>>>
{
    public Guid UserId { get; set; }
}
