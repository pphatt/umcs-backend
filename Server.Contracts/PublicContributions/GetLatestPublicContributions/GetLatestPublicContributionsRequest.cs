using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;

namespace Server.Contracts.PublicContributions.GetLatestPublicContributions;

public class GetLatestPublicContributionsRequest : PaginationRequest
{
    [FromQuery]
    public string? FacultyName { get; set; }

    [FromQuery]
    public string? AcademicYearName { get; set; }
}
