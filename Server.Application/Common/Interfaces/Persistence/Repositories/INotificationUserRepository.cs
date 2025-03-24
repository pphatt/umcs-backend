using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface INotificationUserRepository : IRepository<NotificationUser, Guid>
{
    Task AddUsersNotification(List<NotificationUser> notificationUsers);
}
