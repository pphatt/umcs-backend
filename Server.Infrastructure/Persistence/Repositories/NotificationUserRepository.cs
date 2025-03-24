
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class NotificationUserRepository : RepositoryBase<NotificationUser, Guid>, INotificationUserRepository
{
    private readonly AppDbContext _context;

    public NotificationUserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task AddUsersNotification(List<NotificationUser> notificationUsers)
    {
        await _context.NotificationUsers.AddRangeAsync(notificationUsers);
    }
}
