namespace Server.Application.Common.Dtos.Content.Report.Contributions;

public class GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearMapDto
{
    public String AcademicYear { get; set; }
    public String Faculty { get; set; }
    public float Percentage { get; set; }
}

public class PercentageTotalContributionsPerFacultyPerAcademicYearData
{
    public String Faculty { get; set; }
    public float Percentage { get; set; }
}
