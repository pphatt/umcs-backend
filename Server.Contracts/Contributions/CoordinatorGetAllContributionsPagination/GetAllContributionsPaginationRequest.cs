using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;

namespace Server.Contracts.Contributions.CoordinatorGetAllContributionsPagination;

public class GetAllContributionsPaginationRequest : PaginationRequest
{
    [FromQuery(Name = "academicYear")]
    public string? AcademicYear { get; set; }

    [FromQuery(Name = "faculty")]
    public string? Faculty { get; set; }

    [FromQuery(Name = "status")]
    public string? Status { get; set; }
}
