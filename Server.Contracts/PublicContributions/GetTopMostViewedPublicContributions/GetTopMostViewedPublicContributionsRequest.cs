using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;

namespace Server.Contracts.PublicContributions.GetTopMostViewedPublicContributions;

public class GetTopMostViewedPublicContributionsRequest : PaginationRequest
{
    [FromQuery]
    public string? FacultyName { get; set; }

    [FromQuery]
    public string? AcademicYearName { get; set; }
}
