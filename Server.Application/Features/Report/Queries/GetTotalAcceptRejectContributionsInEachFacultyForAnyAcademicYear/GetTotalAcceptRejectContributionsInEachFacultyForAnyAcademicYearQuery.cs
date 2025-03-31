using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYear;

public class GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearQuery : IRequest<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalAcceptRejectContributionsInEachFacultyForAnyAcademicYearReportDto>>>
{
    public string AcademicYearName { get; set; }
}
