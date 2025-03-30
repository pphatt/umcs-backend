using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetTotalContributorsByEachFacultyForAnyAcademicYear;

public class GetTotalContributorsByEachFacultyForAnyAcademicYearQueryHandler : IRequestHandler<GetTotalContributorsByEachFacultyForAnyAcademicYearQuery, ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsByEachFacultyForAnyAcademicYearReportDto>>>
{
    private readonly IContributionReportService _contributionReportService;

    public GetTotalContributorsByEachFacultyForAnyAcademicYearQueryHandler(IContributionReportService contributionReportService)
    {
        _contributionReportService = contributionReportService;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsByEachFacultyForAnyAcademicYearReportDto>>> Handle(GetTotalContributorsByEachFacultyForAnyAcademicYearQuery request, CancellationToken cancellationToken)
    {
        var result = await _contributionReportService.GetTotalContributorsByEachFacultyForAnyAcademicYear(request.AcademicYearName);

        return result;
    }
}
