using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Commands.UploadUserAvatar;

public class UploadUserAvatarCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid UserId { get; set; }

    public IFormFile Avatar { get; set; }
}
