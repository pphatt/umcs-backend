using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYear;

public class GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearQueryHandler : IRequestHandler<GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearQuery, ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearReportDto>>>
{
    private readonly IContributionReportService _contributionReportService;

    public GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearQueryHandler(IContributionReportService contributionReportService)
    {
        _contributionReportService = contributionReportService;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearReportDto>>> Handle(GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearQuery request, CancellationToken cancellationToken)
    {
        var result = await _contributionReportService.GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearReport(request.AcademicYearName);

        return result;
    }
}
