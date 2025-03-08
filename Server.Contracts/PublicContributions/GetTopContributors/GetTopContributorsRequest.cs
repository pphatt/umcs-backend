using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;
using Server.Domain.Common.Enums;

namespace Server.Contracts.PublicContributions.GetTopContributors;

public class GetTopContributorsRequest : PaginationRequest
{
    [FromQuery(Name = "facultyName")]
    public string? FacultyName { get; set; }

    [FromQuery(Name = "orderBy")]
    public OrderByEnum OrderBy { get; set; }
}
