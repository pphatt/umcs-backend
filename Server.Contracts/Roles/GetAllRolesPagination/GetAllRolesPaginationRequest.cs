namespace Server.Contracts.Roles.GetAllRolesPagination;

public class GetAllRolesPaginationRequest
{
    public string? Keyword { get; set; }

    public int? PageSize { get; set; } = 10;

    public int? PageIndex { get; set; } = 1;
}
