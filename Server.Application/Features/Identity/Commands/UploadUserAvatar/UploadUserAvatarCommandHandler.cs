using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Media;
using Server.Application.Common.Interfaces.Services.Media;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Identity.Commands.UploadUserAvatar;

public class UploadUserAvatarCommandHandler : IRequestHandler<UploadUserAvatarCommand, ErrorOr<ResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMediaService _mediaService;

    public UploadUserAvatarCommandHandler(UserManager<AppUser> userManager, IMediaService mediaService)
    {
        _userManager = userManager;
        _mediaService = mediaService;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(UploadUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        if (user.AvatarPublicId is not null)
        {
            return Errors.User.AvatarAlreadyExist;
        }

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
            Message = "Upload new user's avatar successfully."
        };
    }
}
