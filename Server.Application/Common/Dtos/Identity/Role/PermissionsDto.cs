namespace Server.Application.Common.Dtos.Identity.Role;

public class PermissionsDto
{
    public string RoleId { get; set; } = string.Empty;

    public IList<RoleClaimsDto> RoleClaims { get; set; } = new List<RoleClaimsDto>();
}
