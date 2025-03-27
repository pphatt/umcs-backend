using ErrorOr;

using MediatR;

using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;

namespace Server.Application.Features.Notification.Commands.MarkAllNotificationsAsRed;

public class MarkAllNotificationsAsRedCommandHandler : IRequestHandler<MarkAllNotificationsAsRedCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;

    public MarkAllNotificationsAsRedCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(MarkAllNotificationsAsRedCommand request, CancellationToken cancellationToken)
    {
        var notifications = _unitOfWork.NotificationUserRepository.FindByCondition(x => x.UserId == request.UserId);

        foreach (var notification in notifications)
        {
            notification.HasRed = true;
        }

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Read all notifications successfully."
        };
    }
}
