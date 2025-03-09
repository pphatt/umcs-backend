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

namespace Server.Application.Features.Identity.Commands.EditUserProfile;

public class EditUserProfileCommandHandler : IRequestHandler<EditUserProfileCommand, ErrorOr<ResponseWrapper>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMediaService _mediaService;

    public EditUserProfileCommandHandler(UserManager<AppUser> userManager, IMediaService mediaService)
    {
        _userManager = userManager;
        _mediaService = mediaService;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(EditUserProfileCommand request, CancellationToken cancellationToken)
    {
        // get user by id.
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return Errors.User.CannotFound;
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            user.FirstName = request.FirstName;
        }

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            user.LastName = request.LastName;
        }

        if (request.Dob is not null)
        {
            user.Dob = request.Dob;
        }

        if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
        {
            user.PhoneNumber = request.PhoneNumber;
        }

        // if user change avatar, remove and add to cloudinary.
        if (request.Avatar is not null)
        {
            // remove old avatar if exist.
            var oldAvatar = user.AvatarPublicId;

            if (oldAvatar is not null)
            {
                var deleteAvatar = new DeleteFilesRequest { PublicId = oldAvatar, Type = "Image" };

                await _mediaService.RemoveFilesFromCloudinary(new List<DeleteFilesRequest> { deleteAvatar });
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
        }

        // save the update.
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return result.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Update profile successfully."
        };
    }
}
