using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;
using Server.Domain.Common.Constants.Content;

namespace Server.Contracts.PublicContributions.GetTopMostLikedPublicContributions;

public class GetTopMostLikedPublicContributionsRequest : PaginationRequest
{
    [FromQuery(Name = "facultyName")]
    public string? FacultyName { get; set; }

    [FromQuery(Name = "academicYearName")]
    public string? AcademicYearName { get; set; }
}
