using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Commands.DeleteUserAvatar;

public class DeleteUserAvatarCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid UserId { get; set; }
}
