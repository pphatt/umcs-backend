using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;

namespace Server.Contracts.Contributions.CoordinatorGetAllContributionsPagination;

public class CoordinatorGetAllContributionsPaginationRequest : PaginationRequest
{
    [FromQuery(Name = "academicYear")]
    public string? AcademicYear { get; set; }

    [FromQuery(Name = "status")]
    public string? Status { get; set; } = default!;
}
