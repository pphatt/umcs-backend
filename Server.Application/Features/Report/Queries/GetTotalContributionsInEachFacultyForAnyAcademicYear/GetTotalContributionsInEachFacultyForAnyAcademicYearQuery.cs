using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetTotalContributionsInEachFacultyForAnyAcademicYear;

public class GetTotalContributionsInEachFacultyForAnyAcademicYearQuery : IRequest<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributionsInEachFacultyForAnyAcademicYearReportDto>>>
{
    public String AcademicYearName { get; set; }
}
