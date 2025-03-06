using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Common;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Enums;

namespace Server.Contracts.PublicContributions.GetAllPublicContributionsPagination;

public class GetAllPublicContributionsPaginationRequest : PaginationRequest
{
    [FromQuery(Name = "facultyName")]
    public string? FacultyName { get; set; }

    [FromQuery(Name = "academicYearName")]
    public string? AcademicYearName { get; set; }

    [FromQuery(Name = "allowedGuest")]
    public bool? AllowedGuest { get; set; }

    [FromQuery(Name = "sortBy")]
    public ContributionSortBy? SortBy { get; set; }

    [FromQuery(Name = "orderBy")]
    public ContributionOrderBy OrderBy { get; set; }
}
