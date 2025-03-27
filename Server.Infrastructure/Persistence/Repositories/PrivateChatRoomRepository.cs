
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

    public async Task<PaginationResult<PrivateChatRoomDto>> GetAllChatRoomsPagination(Guid currentUserId, string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        var query = from cr in _context.PrivateChatRooms
                    where cr.DateDeleted == null && (cr.User1Id == currentUserId || cr.User2Id == currentUserId)
                    join u in _context.Users on (cr.User1Id == currentUserId ? cr.User2Id : cr.User1Id) equals u.Id
                    join r in _context.UserRoles on u.Id equals r.UserId
                    select new { cr, u, r };

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
            ChatId = x.cr.Id,
            CurrentUserId = currentUserId,
            ReceiverId = x.u.Id,
            Username = x.u.UserName,
            Avatar = x.u.Avatar,
            LastActivity = x.cr.User2LastActivity,
            LastTimeTexting = x.cr.LastTimeTexting,
            IsOnline = x.u.IsOnline,
            Role = x.r.RoleId.ToString(),
        }).ToListAsync();

        return new PaginationResult<PrivateChatRoomDto>
        {
            CurrentPage = pageIndex,
            RowCount = count,
            PageSize = pageSize,
            Results = result
        };
    }

    public async Task<PaginationResult<PrivateChatMessageDto>> GetUserChatMessages(Guid chatId, Guid currentUserId, int pageIndex = 1, int pageSize = 10)
    {
        var query = from cm in _context.PrivateChatMessages
                    where cm.ChatRoomId == chatId && (cm.SenderId == currentUserId || cm.ReceiverId == currentUserId)
                    orderby cm.DateCreated descending
                    select new { cm };

        var count = await query.CountAsync();

        pageIndex = pageIndex - 1 < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        query = query
            .Skip(skipPage)
            .Take(pageSize);

        var result = await query.Select(x => new PrivateChatMessageDto
        {
            ChatRoomId = x.cm.ChatRoomId,
            SenderId = x.cm.SenderId,
            ReceiverId = x.cm.ReceiverId,
            Content = x.cm.Content,
        }).ToListAsync();

        return new PaginationResult<PrivateChatMessageDto>
        {
            CurrentPage = pageIndex,
            RowCount = count,
            PageSize = pageSize,
            Results = result
        };
    }
}
