using ErrorOr;

using MediatR;

using Microsoft.AspNetCore.Identity;

using Server.Application.Common.Dtos.Content.PrivateChat;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.PrivateChatApp.Queries.GetAllRoomsPagination;

public class GetAllChatRoomsPaginationQueryHandler : IRequestHandler<GetAllChatRoomsPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<PrivateChatRoomDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public GetAllChatRoomsPaginationQueryHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<PrivateChatRoomDto>>>> Handle(GetAllChatRoomsPaginationQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.PrivateChatRoomRepository.GetAllChatRoomsPagination(
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            currentUserId: request.CurrentUserId
        );

        foreach (var room in result.Results)
        {
            var user = await _userManager.FindByIdAsync(room.ReceiverId.ToString());

            var roles = await _userManager.GetRolesAsync(user);

            room.Role = roles[0];
        }

        return new ResponseWrapper<PaginationResult<PrivateChatRoomDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
