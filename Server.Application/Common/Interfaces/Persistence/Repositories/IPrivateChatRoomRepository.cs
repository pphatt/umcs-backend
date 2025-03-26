using Server.Application.Common.Dtos.Content.PrivateChat;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IPrivateChatRoomRepository : IRepository<PrivateChatRoom, Guid>
{
    Task<PaginationResult<PrivateChatRoomDto>> GetAllChatRoomsPagination(Guid userId, string? keyword, int pageIndex = 1, int pageSize = 10);
}
