using Microsoft.AspNetCore.Mvc;
using Server.Domain.Common.Enums;

namespace Server.Contracts.ContributionActivityLogs.GetAllContributionActivityLogsPagination;

public class GetAllContributionActivityLogsPaginationRequest
{
    [FromQuery(Name = "pageIndex")]
    public int PageIndex { get; set; } = 1;

    [FromQuery(Name = "pageSize")]
    public int PageSize { get; set; } = 10;

    [FromQuery(Name = "facultyName")]
    public string? FacultyName { get; set; }

    [FromQuery(Name = "academicYearName")]
    public string? AcademicYearName { get; set; }

    [FromQuery(Name = "orderBy")]
    public OrderByEnum? OrderBy { get; set; }
}
