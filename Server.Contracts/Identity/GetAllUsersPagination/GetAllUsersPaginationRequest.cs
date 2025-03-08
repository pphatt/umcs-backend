using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;
using Server.Domain.Common.Enums;

namespace Server.Contracts.Identity.GetAllUsersPagination;

public class GetAllUsersPaginationRequest : PaginationRequest
{
    [FromQuery(Name = "facultyName")]
    public string? FacultyName { get; set; }

    [FromQuery(Name = "roleName")]
    public string? RoleName { get; set; }

    [FromQuery(Name = "orderBy")]
    public OrderByEnum? OrderBy { get; set; }
}
