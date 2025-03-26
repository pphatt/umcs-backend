using ErrorOr;

using MediatR;

using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.PrivateChat;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Features.PrivateChatApp.Queries.GetAllRoomsPagination;

public class GetAllChatRoomsPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<PrivateChatRoomDto>>>>
{
    public Guid UserId { get; set; }
}
