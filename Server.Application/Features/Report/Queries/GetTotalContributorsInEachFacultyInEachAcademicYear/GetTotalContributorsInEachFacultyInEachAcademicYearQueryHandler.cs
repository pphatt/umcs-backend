using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetTotalContributorsInEachFacultyInEachAcademicYear;

public class GetTotalContributorsInEachFacultyInEachAcademicYearQueryHandler : IRequestHandler<GetTotalContributorsInEachFacultyInEachAcademicYearQuery, ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsInEachFacultyInEachAcademicYearReportDto>>>
{
    private readonly IContributionReportService _contributionReportService;

    public GetTotalContributorsInEachFacultyInEachAcademicYearQueryHandler(IContributionReportService contributionReportService)
    {
        _contributionReportService = contributionReportService;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsInEachFacultyInEachAcademicYearReportDto>>> Handle(GetTotalContributorsInEachFacultyInEachAcademicYearQuery request, CancellationToken cancellationToken)
    {
        var result = await _contributionReportService.GetTotalContributorsInEachFacultyInEachAcademicYearReport();

        return result;
    }
}
