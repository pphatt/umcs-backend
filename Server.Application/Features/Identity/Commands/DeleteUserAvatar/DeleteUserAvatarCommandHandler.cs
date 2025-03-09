using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Services.Media;
using Server.Application.Wrapper;
using Server.Contracts.Common.Media;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Commands.DeleteUserAvatar;

public class DeleteUserAvatarCommandHandler : IRequestHandler<DeleteUserAvatarCommand, ErrorOr<ResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMediaService _mediaService;

    public DeleteUserAvatarCommandHandler(UserManager<AppUser> userManager, IMediaService mediaService)
    {
        _userManager = userManager;
        _mediaService = mediaService;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(DeleteUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        var oldAvatar = user.AvatarPublicId;

        if (oldAvatar is null)
        {
            return Errors.User.AvatarNotFound;
        }

        var deleteAvatar = new DeleteFilesRequest { PublicId = oldAvatar, Type = FileType.Avatar };

        await _mediaService.RemoveFilesFromCloudinary(new List<DeleteFilesRequest> { deleteAvatar });

        user.Avatar = null;
        user.AvatarPublicId = null;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return updateResult.Errors.Select(x => Error.Validation(code: x.Code, description: x.Description)).ToArray();
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Delete avatar successfully."
        };
    }
}
