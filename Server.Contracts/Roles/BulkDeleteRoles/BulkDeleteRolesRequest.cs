namespace Server.Contracts.Roles.BulkDeleteRoles;

public class BulkDeleteRolesRequest
{
    public List<Guid> RoleIds { get; set; }
}
