namespace Server.Contracts.Identity.BulkDeleteUsers;

public class BulkDeleteUsersRequest
{
    public List<Guid> UserIds { get; set; } = default!;
}
