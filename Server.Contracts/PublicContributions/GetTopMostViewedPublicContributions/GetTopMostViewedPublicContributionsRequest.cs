using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;

namespace Server.Contracts.PublicContributions.GetTopMostViewedPublicContributions;

public class GetTopMostViewedPublicContributionsRequest : PaginationRequest
{
    [FromQuery(Name = "facultyName")]
    public string? FacultyName { get; set; }

    [FromQuery(Name = "academicYearName")]
    public string? AcademicYearName { get; set; }
}
