using Microsoft.EntityFrameworkCore;

using Server.Application.Common.Dtos.Content.Notification;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class NotificationRepository : RepositoryBase<Notification, Guid>, INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PaginationResult<NotificationDto>> GetAllUserNotificationsPagination(Guid userId, string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        var query = from n in _context.Notifications
                    where n.DateDeleted == null
                    join nu in _context.NotificationUsers on n.Id equals nu.NotificationId where nu.UserId == userId
                    select new { n, nu };

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.n.Content.Contains(keyword) ||
                                     x.n.Title.Contains(keyword));
        }

        var count = await query.CountAsync();

        pageIndex = pageIndex - 1 < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        query = query
            .OrderByDescending(x => x.n.DateCreated)
            .Skip(skipPage)
            .Take(pageSize);

        var result = await query.Select(x => new NotificationDto
        {
            Id = x.n.Id,
            Title = x.n.Title,
            Content = x.n.Content,
            Type = x.n.Type,
            Username = x.n.Username,
            Avatar = x.n.Avatar,
            HasRed = x.nu.HasRed,
            DateCreated = x.n.DateCreated
        }).ToListAsync();

        return new PaginationResult<NotificationDto>
        {
            CurrentPage = pageIndex,
            RowCount = count,
            PageSize = pageSize,
            Results = result
        };
    }
}
