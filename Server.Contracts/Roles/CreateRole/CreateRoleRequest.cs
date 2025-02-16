namespace Server.Contracts.Roles.CreateRole;

public class CreateRoleRequest
{
    public required string Name { get; set; } = default!;

    public required string DisplayName { get; set; } = default!;
}
