using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetTotalContributorsInEachFacultyInEachAcademicYear;

public class GetTotalContributorsInEachFacultyInEachAcademicYearQuery : IRequest<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsInEachFacultyInEachAcademicYearReportDto>>>
{
}
