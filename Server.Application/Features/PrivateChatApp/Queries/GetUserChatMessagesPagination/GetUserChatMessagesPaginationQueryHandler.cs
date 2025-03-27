
using ErrorOr;

using MediatR;

using Server.Application.Common.Dtos.Content.PrivateChat;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.PrivateChatApp.Queries.GetUserChatMessagesPagination;

public class GetUserChatMessagesPaginationQueryHandler : IRequestHandler<GetUserChatMessagesPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<PrivateChatMessageDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserChatMessagesPaginationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<PrivateChatMessageDto>>>> Handle(GetUserChatMessagesPaginationQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.PrivateChatRoomRepository.GetUserChatMessages(
            chatId: request.ChatId,
            currentUserId: request.CurrentUserId,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize
        );

        return new ResponseWrapper<PaginationResult<PrivateChatMessageDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
