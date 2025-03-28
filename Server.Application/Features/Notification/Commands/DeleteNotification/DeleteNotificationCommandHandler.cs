using ErrorOr;

using MediatR;

using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.Notification.Commands.DeleteNotification;

public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeleteNotificationCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = _unitOfWork.NotificationUserRepository
            .FindByCondition(x => x.NotificationId == request.Id && x.UserId == request.UserId)
            .FirstOrDefault();

        if (notification is null)
        {
            return Errors.Notification.CannotFound;
        }

        notification.DateDeleted = _dateTimeProvider.UtcNow;

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Delete notification successfully."
        };
    }
}
