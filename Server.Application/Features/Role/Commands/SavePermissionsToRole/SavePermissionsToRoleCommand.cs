using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Identity.Role;
using Server.Application.Wrapper;

namespace Server.Application.Features.Role.Commands.SavePermissionsToRole;

public class SavePermissionsToRoleCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public string RoleId { get; set; } = string.Empty;

    public IList<RoleClaimsDto> RoleClaims = new List<RoleClaimsDto>();
}
