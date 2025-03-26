
using Microsoft.EntityFrameworkCore;

using Server.Application.Common.Dtos.Content.PrivateChat;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class PrivateChatRoomRepository : RepositoryBase<PrivateChatRoom, Guid>, IPrivateChatRoomRepository
{
    private readonly AppDbContext _context;

    public PrivateChatRoomRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PaginationResult<PrivateChatRoomDto>> GetAllChatRoomsPagination(Guid userId, string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        var query = from cr in _context.PrivateChatRooms
                    where cr.DateDeleted == null
                    join u in _context.Users on cr.User2Id equals u.Id
                    select new { cr, u };

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.u.UserName.Contains(keyword));
        }

        var count = await query.CountAsync();

        pageIndex = pageIndex - 1 < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        query = query
            .OrderByDescending(x => x.cr.LastTimeTexting)
            .Skip(skipPage)
            .Take(pageSize);

        var result = await query.Select(x => new PrivateChatRoomDto
        {
            UserId = x.cr.User2Id,
            Username = x.u.UserName,
            Avatar = x.u.Avatar,
            LastActivity = x.cr.User2LastActivity,
            LastTimeTexting = x.cr.LastTimeTexting
        }).ToListAsync();

        return new PaginationResult<PrivateChatRoomDto>
        {
            CurrentPage = pageIndex,
            RowCount = count,
            PageSize = pageSize,
            Results = result
        };
    }
}
