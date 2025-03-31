namespace Server.Application.Common.Dtos.Content.Report.Contributions;

public class GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearDto
{
    public String AcademicYear { get; set; }
    public String Faculty { get; set; }
    public float Percentage { get; set; }
}

public class PercentageTotalContributionsPerFacultyPerAcademicYearReportDto
{
    public String Faculty { get; set; }
    public float Percentage { get; set; }
}
