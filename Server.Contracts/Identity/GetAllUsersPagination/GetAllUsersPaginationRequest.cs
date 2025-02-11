namespace Server.Contracts.Identity.GetAllUsersPagination;

public class GetAllUsersPaginationRequest
{
    public int? PageIndex { get; set; } = 1;

    public int? PageSize { get; set; } = 10;
}
