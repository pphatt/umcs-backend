using ErrorOr;

using MediatR;

using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;

namespace Server.Application.Features.Notification.Commands.UnreadAllNotifications;

public class UnreadAllNotificationsCommandHandler : IRequestHandler<UnreadAllNotificationsCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnreadAllNotificationsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(UnreadAllNotificationsCommand request, CancellationToken cancellationToken)
    {
        var notifications = _unitOfWork.NotificationUserRepository
            .FindByCondition(x => x.UserId == request.UserId);

        foreach (var notification in notifications)
        {
            notification.HasRed = false;
        }

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Unread all notifications successfully."
        };
    }
}
