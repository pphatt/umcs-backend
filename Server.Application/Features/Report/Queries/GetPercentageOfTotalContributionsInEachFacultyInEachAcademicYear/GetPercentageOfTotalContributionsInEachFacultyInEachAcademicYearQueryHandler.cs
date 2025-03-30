using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYear;

public class GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearQueryHandler : IRequestHandler<GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearQuery, ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearReportDto>>>
{
    private readonly IContributionReportService _contributionReportService;

    public GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearQueryHandler(IContributionReportService contributionReportService)
    {
        _contributionReportService = contributionReportService;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearReportDto>>> Handle(GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearQuery request, CancellationToken cancellationToken)
    {
        var result = await _contributionReportService.GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYear();

        return result;
    }
}
