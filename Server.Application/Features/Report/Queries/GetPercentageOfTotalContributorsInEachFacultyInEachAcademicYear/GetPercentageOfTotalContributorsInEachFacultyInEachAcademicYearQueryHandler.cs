using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYear;

public class GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearQueryHandler : IRequestHandler<GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearQuery, ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearReportDto>>>
{
    private readonly IContributionReportService _contributionReportService;

    public GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearQueryHandler(IContributionReportService contributionReportService)
    {
        _contributionReportService = contributionReportService;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearReportDto>>> Handle(GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearQuery request, CancellationToken cancellationToken)
    {
        var result = await _contributionReportService.GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearReport();

        return result;
    }
}
