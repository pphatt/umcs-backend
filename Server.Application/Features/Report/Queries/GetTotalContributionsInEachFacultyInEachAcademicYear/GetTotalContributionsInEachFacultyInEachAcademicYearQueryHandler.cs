using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetTotalContributionsInEachFacultyInEachAcademicYear;

public class GetTotalContributionsInEachFacultyInEachAcademicYearQueryHandler : IRequestHandler<GetTotalContributionsInEachFacultyInEachAcademicYearQuery, ReportResponseWrapper<AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>>>
{
    private readonly IContributionReportService _contributionReportService;

    public GetTotalContributionsInEachFacultyInEachAcademicYearQueryHandler(IContributionReportService contributionReportService)
    {
        _contributionReportService = contributionReportService;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>>> Handle(GetTotalContributionsInEachFacultyInEachAcademicYearQuery request, CancellationToken cancellationToken)
    {
        var result = await _contributionReportService.GetTotalContributionsInEachFacultyInEachAcademicYearReport();

        return result;
    }
}
