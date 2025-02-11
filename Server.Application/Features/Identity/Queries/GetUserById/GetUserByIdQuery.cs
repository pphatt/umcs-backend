using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<ErrorOr<ResponseWrapper<UserDto>>>
{
    public Guid Id { get; set; }
}
