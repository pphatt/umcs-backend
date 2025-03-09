using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Commands.ChangeUserAvatar;

public class ChangeUserAvatarCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid UserId { get; set; }

    public IFormFile Avatar { get; set; }
}
