using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetTotalContributionsInEachFacultyForAnyAcademicYear;

public class GetTotalContributionsInEachFacultyForAnyAcademicYearQueryHandler : IRequestHandler<GetTotalContributionsInEachFacultyForAnyAcademicYearQuery, ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributionsInEachFacultyForAnyAcademicYearReportDto>>>
{
    private readonly IContributionReportService _contributionReportService;

    public GetTotalContributionsInEachFacultyForAnyAcademicYearQueryHandler(IContributionReportService contributionReportService)
    {
        _contributionReportService = contributionReportService;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributionsInEachFacultyForAnyAcademicYearReportDto>>> Handle(GetTotalContributionsInEachFacultyForAnyAcademicYearQuery request, CancellationToken cancellationToken)
    {
        var result = await _contributionReportService.GetTotalContributionsInEachFacultyForAnyAcademicYearReport(request.AcademicYearName);

        return result;
    }
}
