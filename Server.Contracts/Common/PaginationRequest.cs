using Microsoft.AspNetCore.Mvc;

namespace Server.Contracts.Common;

public class PaginationRequest
{
    [FromQuery(Name = "keyword")]
    public string? Keyword { get; set; }

    [FromQuery(Name = "pageIndex")]
    public int PageIndex { get; set; } = 1;

    [FromQuery(Name = "pageSize")]
    public int PageSize { get; set; } = 10;
}
