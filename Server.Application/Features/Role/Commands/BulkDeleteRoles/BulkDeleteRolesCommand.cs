using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.Role.Commands.BulkDeleteRoles;

public class BulkDeleteRolesCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public List<Guid> RoleIds { get; set; } = default!;
}
