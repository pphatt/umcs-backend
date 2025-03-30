namespace Server.Application.Common.Dtos.Content.Report.Contributions;

public class GetTotalContributionsInEachFacultyInEachAcademicYearDto
{
    public string AcademicYear { get; set; }
    public string Faculty { get; set; }
    public int TotalContributions { get; set; }
}

public class TotalContributionsInEachFacultyInEachAcademicYearReportDto
{
    public String Faculty { get; set; }
    public int TotalContributions { get; set; }
}
