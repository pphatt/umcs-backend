using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Server.Application.Common.Extensions;
using Server.Application.Features.PrivateChatApp.Commands.SendChatMessage;
using Server.Application.Features.PrivateChatApp.Queries.GetAllRoomsPagination;
using Server.Application.Features.PrivateChatApp.Queries.GetUserChatMessagesPagination;
using Server.Contracts.PrivateChats.GetAllChatRoomsPagination;
using Server.Contracts.PrivateChats.GetUserChatMessagesPagination;
using Server.Contracts.PrivateChats.SendChatMessage;

namespace Server.Api.Controllers.ClientApi;

public class PrivateChatController : ClientApiController
{
    private readonly IMapper _mapper;

    public PrivateChatController(IMapper mapper, ISender mediatorSender) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] SendChatMessageRequest request)
    {
        var mapper = _mapper.Map<SendChatMessageCommand>(request);

        mapper.SenderId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            sendResult => Ok(sendResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("get-all-chat-rooms-pagination")]
    public async Task<IActionResult> GetAllChatRoomsPagination([FromQuery] GetAllChatRoomsPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllChatRoomsPaginationQuery>(request);

        mapper.CurrentUserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("get-user-message-pagination")]
    public async Task<IActionResult> GetUserMessagesPagination([FromQuery] GetUserChatMessagesPaginationRequest request)
    {
        var mapper = _mapper.Map<GetUserChatMessagesPaginationQuery>(request);

        mapper.CurrentUserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }
}
