using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYear;

public class GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearQueryHandler : IRequestHandler<GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearQuery, ReportResponseWrapper<AcademicYearReportResponseWrapper<PercentageTotalContributionsPerFacultyPerAcademicYearReportDto>>>
{
    private readonly IContributionReportService _contributionReportService;

    public GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearQueryHandler(IContributionReportService contributionReportService)
    {
        _contributionReportService = contributionReportService;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<PercentageTotalContributionsPerFacultyPerAcademicYearReportDto>>> Handle(GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearQuery request, CancellationToken cancellationToken)
    {
        var result = await _contributionReportService.GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYear(request.AcademicYearName);

        return result;
    }
}
