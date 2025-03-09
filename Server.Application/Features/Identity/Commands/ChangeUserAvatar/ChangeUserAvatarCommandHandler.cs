using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Media;
using Server.Application.Common.Interfaces.Services.Media;
using Server.Application.Wrapper;
using Server.Contracts.Common.Media;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Commands.ChangeUserAvatar;

public class ChangeUserAvatarCommandHandler : IRequestHandler<ChangeUserAvatarCommand, ErrorOr<ResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMediaService _mediaService;

    public ChangeUserAvatarCommandHandler(UserManager<AppUser> userManager, IMediaService mediaService)
    {
        _userManager = userManager;
        _mediaService = mediaService;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(ChangeUserAvatarCommand request, CancellationToken cancellationToken)
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

        await _mediaService.RemoveFilesFromCloudinary(
            new List<DeleteFilesRequest> { deleteAvatar }
        );

        var uploadResult = await _mediaService.UploadFilesToCloudinary(
            new List<IFormFile> { request.Avatar },
            new FileRequiredParamsDto { type = FileType.Avatar, userId = request.UserId }
        );

        foreach (var r in uploadResult)
        {
            user.Avatar = r.Path;
            user.AvatarPublicId = r.PublicId;
        }

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return result.Errors.Select(x => Error.Failure(code: x.Code, description: x.Description)).ToArray();
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Change user's avatar successfully."
        };
    }
}
