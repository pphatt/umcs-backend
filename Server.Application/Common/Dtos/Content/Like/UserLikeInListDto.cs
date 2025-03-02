using Server.Application.Common.Dtos.Identity.Users;

namespace Server.Application.Common.Dtos.Content.Like;

public class UserLikeInListDto : UserInListDto
{
    public DateTime DateCreated { get; set; }
}
