using ErrorOr;

using MediatR;

using Server.Application.Common.Dtos.Content.Notification;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.Notification.Queries.GetAllUserNotificationsPagination;

public class GetAllUserNotificationsPaginationQueryHandler : IRequestHandler<GetAllUserNotificationsPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<NotificationDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUserNotificationsPaginationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<NotificationDto>>>> Handle(GetAllUserNotificationsPaginationQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.NotificationRepository.GetAllUserNotificationsPagination(
            userId: request.UserId,
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize
        );

        return new ResponseWrapper<PaginationResult<NotificationDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
