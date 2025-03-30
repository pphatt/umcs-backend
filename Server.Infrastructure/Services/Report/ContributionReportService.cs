using Dapper;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;
using Server.Infrastructure.Migrations;
using Server.Infrastructure.Persistence.AppDbConnection;

namespace Server.Infrastructure.Services.Report;

public class ContributionReportService : IContributionReportService
{
    private readonly IAppDbConnectionFactory _connectionFactory;

    public ContributionReportService(IAppDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>>> GetTotalContributionsInEachFacultyInEachAcademicYearReport()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql =
            """
            SELECT ay.Name AS AcademicYear,
                f.Name AS Faculty,
                COALESCE(count(c.Id), 0) AS TotalContributions
            FROM AcademicYears ay
            CROSS JOIN Faculties f
            LEFT JOIN Contributions c ON c.AcademicYearId = ay.Id AND c.FacultyId = f.Id
            WHERE c.DateDeleted is null
            GROUP BY ay.Name, f.Name
            ORDER BY ay.Name, f.Name;
            """;

        var query = await connection.QueryAsync<GetTotalContributionsInEachFacultyInEachAcademicYearDto>(sql: sql);

        var setOfAcademicYears = query.Select(x => x.AcademicYear).ToHashSet();
        var setOfFaculties = query.Select(x => x.Faculty).ToHashSet();

        var result = new ReportResponseWrapper<AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>>();

        Parallel.ForEach(setOfAcademicYears, academicYear =>
        {
            var academicYearResult = new AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>();

            academicYearResult.AcademicYear = academicYear;

            Parallel.ForEach(setOfFaculties, faculty =>
            {
                var facultyResult = new TotalContributionsInEachFacultyInEachAcademicYearReportDto();

                var contributionCount = query
                    .Where(x => x.AcademicYear == academicYear && x.Faculty == faculty)
                    .FirstOrDefault()!
                    .TotalContributions;

                facultyResult.Faculty = faculty;
                facultyResult.TotalContributions = contributionCount;

                academicYearResult.DataSets.Add(facultyResult);
            });

            result.Response.Add(academicYearResult);
        });

        result.Response = result.Response
            .OrderByDescending(x => x.AcademicYear)
            .ToList();

        return result;
    }
}
