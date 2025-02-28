using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;

namespace Server.Contracts.Contributions.GetAllUngradedContributionsPagination;

public class GetAllUngradedContributionsPaginationRequest : PaginationRequest
{
    [FromQuery(Name = "academicYearName")]
    public string? AcademicYear { get; set; }

    [FromQuery(Name = "facultyName")]
    public string? Faculty { get; set; }
}
