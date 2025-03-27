using ErrorOr;

using MediatR;

using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.PrivateChat;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.PrivateChatApp.Queries.GetUserChatMessagesPagination;

public class GetUserChatMessagesPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<PrivateChatMessageDto>>>>
{
    public Guid ChatId { get; set; }

    public Guid CurrentUserId { get; set; }
}
