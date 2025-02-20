using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Identity.Role;
using Server.Application.Wrapper;

namespace Server.Application.Features.Role.Queries.GetAllRolePermissions;

public class GetAllRolePermissionsQuery : IRequest<ErrorOr<ResponseWrapper<PermissionsDto>>>
{
    public Guid Id { get; set; }
}
