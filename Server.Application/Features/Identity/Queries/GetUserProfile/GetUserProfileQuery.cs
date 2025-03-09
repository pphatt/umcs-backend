using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Queries.GetUserProfile;

public class GetUserProfileQuery : IRequest<ErrorOr<ResponseWrapper<UserProfileDto>>>
{
    public Guid UserId { get; set; }
}
