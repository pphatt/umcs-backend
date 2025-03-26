using ErrorOr;

using MediatR;

using Server.Application.Common.Dtos.Content.PrivateChat;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.PrivateChatApp.Queries.GetAllRoomsPagination;

public class GetAllChatRoomsPaginationQueryHandler : IRequestHandler<GetAllChatRoomsPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<PrivateChatRoomDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllChatRoomsPaginationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<PrivateChatRoomDto>>>> Handle(GetAllChatRoomsPaginationQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.PrivateChatRoomRepository.GetAllChatRoomsPagination(
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            userId: request.UserId
        );

        return new ResponseWrapper<PaginationResult<PrivateChatRoomDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
