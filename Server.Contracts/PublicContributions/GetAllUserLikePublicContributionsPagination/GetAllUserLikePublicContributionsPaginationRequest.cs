using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;
using Server.Domain.Common.Enums;

namespace Server.Contracts.PublicContributions.GetAllUserLikePublicContributionsPagination;

public class GetAllUserLikePublicContributionsPaginationRequest : PaginationRequest
{
    [FromQuery(Name = "facultyName")]
    public string? FacultyName { get; set; }

    [FromQuery(Name = "academicYearName")]
    public string? AcademicYearName { get; set; }

    [FromQuery(Name = "orderBy")]
    public OrderByEnum OrderBy { get; set; }
}
