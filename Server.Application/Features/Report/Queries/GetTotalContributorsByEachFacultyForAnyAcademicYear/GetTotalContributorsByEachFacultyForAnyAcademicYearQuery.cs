using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetTotalContributorsByEachFacultyForAnyAcademicYear;

public class GetTotalContributorsByEachFacultyForAnyAcademicYearQuery : IRequest<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsByEachFacultyForAnyAcademicYearReportDto>>>
{
    public string AcademicYearName { get; set; }
}
