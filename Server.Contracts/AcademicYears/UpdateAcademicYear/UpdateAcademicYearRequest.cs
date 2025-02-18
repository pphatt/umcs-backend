using Microsoft.AspNetCore.Mvc;

namespace Server.Contracts.AcademicYears.UpdateAcademicYear;

public class UpdateAcademicYearRequest
{
    [FromRoute]
    public Guid Id { get; set; }

    [FromForm]
    public string AcademicYearName { get; set; } = default!;

    [FromForm]
    public required DateTime StartClosureDate { get; set; }

    [FromForm]
    public required DateTime EndClosureDate { get; set; }

    [FromForm]
    public required DateTime FinalClosureDate { get; set; }

    [FromForm]
    public required bool IsActive { get; set; } = false;
}
