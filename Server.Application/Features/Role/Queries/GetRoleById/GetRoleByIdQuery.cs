using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Identity.Role;
using Server.Application.Wrapper;

namespace Server.Application.Features.Role.Queries.GetRoleById;

public class GetRoleByIdQuery : IRequest<ErrorOr<ResponseWrapper<RoleDto>>>
{
    public Guid Id { get; set; }
}
