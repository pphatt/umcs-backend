using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Wrapper.Report;

namespace Server.Application.Common.Interfaces.Services.Report;

public interface IContributionReportService
{
    Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>>> GetTotalContributionsInEachFacultyInEachAcademicYearReport();
    Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<PercentageTotalContributionsPerFacultyPerAcademicYearData>>> GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYear(string academicYear);
}
