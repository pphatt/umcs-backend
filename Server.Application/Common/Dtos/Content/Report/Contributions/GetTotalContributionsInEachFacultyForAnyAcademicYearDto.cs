namespace Server.Application.Common.Dtos.Content.Report.Contributions;

public class GetTotalContributionsInEachFacultyForAnyAcademicYearDto
{
    public string AcademicYear { get; set; }
    public string Faculty { get; set; }
    public int TotalContributions { get; set; }
}

public class GetTotalContributionsInEachFacultyForAnyAcademicYearReportDto
{
    public String Faculty { get; set; }
    public int TotalContributions { get; set; }
}
