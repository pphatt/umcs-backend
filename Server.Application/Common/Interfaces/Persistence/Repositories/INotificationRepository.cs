using Server.Application.Common.Dtos.Content.Notification;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface INotificationRepository : IRepository<Notification, Guid>
{
    Task<PaginationResult<NotificationDto>> GetAllUserNotificationsPagination(Guid userId, string? keyword, int pageIndex = 1, int pageSize = 10);
}
