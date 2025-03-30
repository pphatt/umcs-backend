using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetTotalContributionsInEachFacultyInEachAcademicYear;

public class GetTotalContributionsInEachFacultyInEachAcademicYearQuery : IRequest<ReportResponseWrapper<AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>>>
{
}
