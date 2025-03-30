namespace Server.Application.Common.Dtos.Content.Report.Contributions;

public class GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearDto
{
    public String AcademicYear { get; set; }
    public String Faculty { get; set; }
    public float Percentage { get; set; }
}

public class GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearReportDto
{
    public String Faculty { get; set; }
    public float Percentage { get; set; }
}
