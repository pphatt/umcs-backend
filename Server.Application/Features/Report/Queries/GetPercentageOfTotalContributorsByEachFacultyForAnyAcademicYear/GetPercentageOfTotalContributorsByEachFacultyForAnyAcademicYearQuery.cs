using MediatR;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Wrapper.Report;

namespace Server.Application.Features.Report.Queries.GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYear;

public class GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearQuery : IRequest<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearReportDto>>>
{
    public string AcademicYearName { get; set; }
}
