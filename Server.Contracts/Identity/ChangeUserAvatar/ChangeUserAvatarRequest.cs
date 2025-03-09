using Microsoft.AspNetCore.Http;

namespace Server.Contracts.Identity.ChangeUserAvatar;

public class ChangeUserAvatarRequest
{
    public IFormFile Avatar { get; set; }
}
