using ErrorOr;

using MediatR;

using Server.Application.Wrapper;

namespace Server.Application.Features.PrivateChatApp.Commands.SendChatMessage;

public class SendChatMessageCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Content { get; set; }
}
