using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Server.Application.Common.Extensions;
using Server.Application.Features.PrivateChatApp.Queries.GetAllRoomsPagination;
using Server.Contracts.PrivateChatRooms.GetAllChatRoomsPagination;

namespace Server.Api.Controllers.ClientApi;

public class PrivateChatController : ClientApiController
{
    private readonly IMapper _mapper;

    public PrivateChatController(IMapper mapper, ISender mediatorSender) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpGet("get-all-chat-rooms-pagination")]
    public async Task<IActionResult> GetAllChatRoomsPagination([FromQuery] GetAllChatRoomsPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllChatRoomsPaginationQuery>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }
}
