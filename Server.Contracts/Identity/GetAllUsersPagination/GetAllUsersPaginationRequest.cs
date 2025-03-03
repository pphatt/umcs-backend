using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;

namespace Server.Contracts.Identity.GetAllUsersPagination;

public class GetAllUsersPaginationRequest : PaginationRequest
{
    [FromQuery]
    public string? FacultyName { get; set; }

    [FromQuery]
    public string? RoleName { get; set; }
}
