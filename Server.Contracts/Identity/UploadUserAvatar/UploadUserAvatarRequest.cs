using Microsoft.AspNetCore.Http;

namespace Server.Contracts.Identity.UploadUserAvatar;

public class UploadUserAvatarRequest
{
    public IFormFile Avatar { get; set; }
}
