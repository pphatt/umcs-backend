namespace Server.Contracts.AcademicYears.CreateAcademicYear;

public class CreateAcademicYearRequest
{
    public required string Name { get; set; }

    public required DateTime StartClosureDate { get; set; }

    public required DateTime EndClosureDate { get; set; }

    public required DateTime FinalClosureDate { get; set; }

    public required bool IsActive { get; set; } = false;
}
