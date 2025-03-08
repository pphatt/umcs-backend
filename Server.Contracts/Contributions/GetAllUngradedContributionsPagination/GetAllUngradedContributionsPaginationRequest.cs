using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;
using Server.Domain.Common.Enums;

namespace Server.Contracts.Contributions.GetAllUngradedContributionsPagination;

public class GetAllUngradedContributionsPaginationRequest : PaginationRequest
{
    [FromQuery(Name = "academicYearName")]
    public string? AcademicYear { get; set; }

    [FromQuery(Name = "facultyName")]
    public string? Faculty { get; set; }

    [FromQuery(Name = "orderBy")]
    public OrderByEnum? OrderBy { get; set; }
}
