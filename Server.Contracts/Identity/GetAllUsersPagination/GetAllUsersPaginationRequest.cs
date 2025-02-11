namespace Server.Contracts.Identity.GetAllUsersPagination;

public class GetAllUsersPaginationRequest
{
    public string? Keyword { get; set; }

    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
