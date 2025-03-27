using Microsoft.EntityFrameworkCore;

using Server.Application.Common.Dtos.Content.PrivateChat;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class PrivateChatMessageRepository : RepositoryBase<PrivateChatMessage, Guid>, IPrivateChatMessageRepository
{
    private readonly AppDbContext _context;

    public PrivateChatMessageRepository(AppDbContext context) : base(context)
    {
        _context = context;
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
