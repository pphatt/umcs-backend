namespace Server.Contracts.Roles.UpdateRole;

public class UpdateRoleRequest
{
    public required Guid Id { get; set; }

    public required string Name { get; set; } = default!;

    public required string DisplayName { get; set; } = default!;
}
