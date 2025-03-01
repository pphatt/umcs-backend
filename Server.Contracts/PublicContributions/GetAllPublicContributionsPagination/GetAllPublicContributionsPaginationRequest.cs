using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;

namespace Server.Contracts.PublicContributions.GetAllPublicContributionsPagination;

public class GetAllPublicContributionsPaginationRequest : PaginationRequest
{
    [FromQuery(Name = "academicYearName")]
    public string? AcademicYearName { get; set; }

    [FromQuery(Name = "facultyName")]
    public string? FacultyName { get; set; }

    [FromQuery(Name = "allowedGuest")]
    public bool? AllowedGuest { get; set; }
}
