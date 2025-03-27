using Server.Application.Common.Dtos.Content.PrivateChat;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IPrivateChatMessageRepository : IRepository<PrivateChatMessage, Guid>
{
    Task<PaginationResult<PrivateChatMessageDto>> GetUserChatMessages(Guid chatId, Guid currentUserId, int pageIndex = 1, int pageSize = 10);
}
