using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Identity.Users;

namespace Server.Application.Features.Identity.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<ErrorOr<UserDto>>
{
    public Guid Id { get; set; }
}
