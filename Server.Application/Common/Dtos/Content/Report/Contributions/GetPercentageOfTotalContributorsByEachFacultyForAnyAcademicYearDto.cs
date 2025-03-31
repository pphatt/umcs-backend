namespace Server.Application.Common.Dtos.Content.Report.Contributions;

public class GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearDto
{
    public String AcademicYear { get; set; }
    public String Faculty { get; set; }
    public float Percentage { get; set; }
}

public class GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearReportDto
{
    public String Faculty { get; set; }
    public float Percentage { get; set; }
}
