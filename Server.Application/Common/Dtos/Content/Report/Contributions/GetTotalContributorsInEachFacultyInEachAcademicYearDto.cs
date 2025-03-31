namespace Server.Application.Common.Dtos.Content.Report.Contributions;

public class GetTotalContributorsInEachFacultyInEachAcademicYearDto
{
    public String AcademicYear { get; set; }
    public String Faculty { get; set; }
    public int Contributors { get; set; }
}

public class GetTotalContributorsInEachFacultyInEachAcademicYearReportDto
{
    public String Faculty { get; set; }
    public int Contributors { get; set; }
}
