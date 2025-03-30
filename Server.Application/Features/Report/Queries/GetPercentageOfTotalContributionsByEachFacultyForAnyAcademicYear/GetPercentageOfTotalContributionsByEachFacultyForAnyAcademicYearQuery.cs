using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYear;

public class GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearQuery : IRequest<ReportResponseWrapper<AcademicYearReportResponseWrapper<PercentageTotalContributionsPerFacultyPerAcademicYearData>>>
{
    public string AcademicYearName { get; set; }
}
