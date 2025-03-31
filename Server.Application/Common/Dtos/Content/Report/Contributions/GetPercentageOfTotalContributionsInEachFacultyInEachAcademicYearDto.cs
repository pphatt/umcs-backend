namespace Server.Application.Common.Dtos.Content.Report.Contributions;

public class GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearDto
{
    public string AcademicYear { get; set; }
    public string Faculty { get; set; }
    public float Percentage { get; set; }
}

public class GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearReportDto
{
    public string Faculty { get; set; }
    public float Percentage { get; set; }
}
