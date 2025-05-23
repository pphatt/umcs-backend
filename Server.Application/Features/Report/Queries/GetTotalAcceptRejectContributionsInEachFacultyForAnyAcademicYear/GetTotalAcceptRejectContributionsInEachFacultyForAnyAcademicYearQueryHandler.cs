﻿using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYear;

public class GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearQueryHandler : IRequestHandler<GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearQuery, ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearReportDto>>>
{
    private readonly IContributionReportService _contributionReportService;

    public GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearQueryHandler(IContributionReportService contributionReportService)
    {
        _contributionReportService = contributionReportService;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearReportDto>>> Handle(GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearQuery request, CancellationToken cancellationToken)
    {
        var result = await _contributionReportService.GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearReport(request.AcademicYearName);

        return result;
    }
}
