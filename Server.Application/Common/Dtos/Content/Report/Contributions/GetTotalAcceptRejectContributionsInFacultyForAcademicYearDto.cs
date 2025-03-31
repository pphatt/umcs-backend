namespace Server.Application.Common.Dtos.Content.Report.Contributions;

public class GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearDto
{
    public String AcademicYear { get; set; }
    public String Faculty { get; set; }
    public int AcceptedContributions { get; set; }
    public int RejectedContributions { get; set; }
    public int TotalContributions { get; set; }
}

public class GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearReportDto
{
    public String Faculty { get; set; }
    public int AcceptedContributions { get; set; }
    public int RejectedContributions { get; set; }
    public int TotalContributions { get; set; }
}
