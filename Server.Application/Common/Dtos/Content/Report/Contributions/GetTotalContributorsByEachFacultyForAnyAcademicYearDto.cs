namespace Server.Application.Common.Dtos.Content.Report.Contributions;

public class GetTotalContributorsByEachFacultyForAnyAcademicYearDto
{
    public String AcademicYear { get; set; }
    public String Faculty { get; set; }
    public int Contributors { get; set; }
}

public class GetTotalContributorsByEachFacultyForAnyAcademicYearReportDto
{
    public String Faculty { get; set; }
    public int Contributors { get; set; }
}
