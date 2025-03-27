using ErrorOr;

using MediatR;

using Server.Application.Wrapper;

namespace Server.Application.Features.PrivateChatApp.Commands.MarkMessageAsRed;

public class MarkMessageAsRedCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid ChatId { get; set; }
    public Guid CurrentUserId { get; set; }
}
