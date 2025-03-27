using Server.Contracts.Common;

namespace Server.Contracts.PrivateChats.GetUserChatMessagesPagination;

public class GetUserChatMessagesPaginationRequest : PaginationRequest
{
    public Guid ChatId { get; set; }
}
