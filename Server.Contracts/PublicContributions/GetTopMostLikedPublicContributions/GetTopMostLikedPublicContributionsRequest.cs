using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;

namespace Server.Contracts.PublicContributions.GetTopMostLikedPublicContributions;

public class GetTopMostLikedPublicContributionsRequest : PaginationRequest
{
    [FromQuery]
    public string? FacultyName { get; set; }

    [FromQuery]
    public string? AcademicYearName { get; set; }
}
