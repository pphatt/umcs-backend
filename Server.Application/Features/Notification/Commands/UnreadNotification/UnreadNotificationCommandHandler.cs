using ErrorOr;

using MediatR;

using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.Notification.Commands.UnreadNotification;

public class UnreadNotificationCommandHandler : IRequestHandler<UnreadNotificationCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnreadNotificationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(UnreadNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = _unitOfWork.NotificationUserRepository
            .FindByCondition(x => x.NotificationId == request.NotificationId && x.UserId == request.UserId)
            .FirstOrDefault();

        if (notification is null)
        {
            return Errors.Notification.CannotFound;
        }

        notification.HasRed = false;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Unread notification successfully."
        };
    }
}
