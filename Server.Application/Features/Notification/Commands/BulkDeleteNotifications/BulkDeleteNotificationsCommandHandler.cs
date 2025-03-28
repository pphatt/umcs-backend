
using ErrorOr;

using MediatR;

using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.Notification.Commands.BulkDeleteNotifications;

public class BulkDeleteNotificationsCommandHandler : IRequestHandler<BulkDeleteNotificationsCommand, ErrorOr<ResponseWrapper>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BulkDeleteNotificationsCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(BulkDeleteNotificationsCommand request, CancellationToken cancellationToken)
    {
        var notificationIds = request.Ids;
        var successfullyDeletedItems = new List<Guid>();

        foreach (var id in notificationIds)
        {
            var notification = _unitOfWork.NotificationUserRepository
                .FindByCondition(x => x.NotificationId == id && x.UserId == request.UserId)
                .FirstOrDefault();

            if (notification is null)
            {
                return Errors.Notification.CannotFound;
            }

            notification.DateDeleted = _dateTimeProvider.UtcNow;

            successfullyDeletedItems.Add(notification.Id);
        }

        await _unitOfWork.CompleteAsync();

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Messages = new List<string>
            {
                $"Successfully deleted {successfullyDeletedItems.Count} notifications.",
                "Each item is available for recovery."
            }
        };
    }
}
