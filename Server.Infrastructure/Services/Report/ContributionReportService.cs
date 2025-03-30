using Dapper;

using Server.Application.Common.Dtos.Content.Report.Contributions;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Common.Interfaces.Services.Report;
using Server.Application.Wrapper.Report;
using Server.Infrastructure.Persistence.AppDbConnection;

namespace Server.Infrastructure.Services.Report;

public class ContributionReportService : IContributionReportService
{
    private readonly IAppDbConnectionFactory _connectionFactory;
    private readonly IFacultyRepository _facultyRepository;

    public ContributionReportService(IAppDbConnectionFactory connectionFactory, IFacultyRepository facultyRepository)
    {
        _connectionFactory = connectionFactory;
        _facultyRepository = facultyRepository;
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

        var query = (await connection.QueryAsync<GetTotalContributionsInEachFacultyInEachAcademicYearDto>(sql: sql)).AsList();

        var count = query.Count();
        var facultyCount = await _facultyRepository.CountAsync();

        var result = new ReportResponseWrapper<AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>>();

        for (var i = 0; i < count; i += facultyCount)
        {
            var academicYearResult = new AcademicYearReportResponseWrapper<TotalContributionsInEachFacultyInEachAcademicYearReportDto>();

            academicYearResult.AcademicYear = query[i].AcademicYear;

            for (var j = i; j < i + facultyCount; j++)
            {
                var facultyResult = new TotalContributionsInEachFacultyInEachAcademicYearReportDto();

                facultyResult.Faculty = query[j].Faculty;
                facultyResult.TotalContributions = query[j].TotalContributions;

                academicYearResult.DataSets.Add(facultyResult);
            }

            result.Response.Add(academicYearResult);
        }

        result.Response = result.Response
            .OrderByDescending(x => x.AcademicYear)
            .ToList();

        return result;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<PercentageTotalContributionsPerFacultyPerAcademicYearReportDto>>> GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYear(string academicYearName)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql =
            """
            SELECT
                ay.Name AS AcademicYear,
                cc.FacultyName AS Faculty,
                CAST((cc.ContributionCount * 1.0 / COALESCE(tc.TotalCount, 0)) * 100 AS INT) AS Percentage
            FROM AcademicYears ay
            CROSS JOIN (
                SELECT
                    f.Name AS FacultyName,
                    COUNT(c.Id) AS ContributionCount
                FROM Faculties f
                LEFT JOIN Contributions c ON f.Id = c.FacultyId
                    AND c.AcademicYearId = (SELECT Id FROM AcademicYears WHERE Name = @academicYearName)
                    AND c.DateDeleted IS NULL
                WHERE f.DateDeleted IS NULL
                GROUP BY f.Name
            ) AS cc
            CROSS JOIN (
                SELECT
                    COUNT(*) AS TotalCount
                FROM Contributions c
                JOIN AcademicYears ay ON c.AcademicYearId = ay.Id
                JOIN Faculties f ON c.FacultyId = f.Id
                WHERE ay.Name = @academicYearName AND c.DateDeleted IS NULL AND f.DateDeleted IS NULL
            ) AS tc
            WHERE ay.Name = @academicYearName
            ORDER BY cc.FacultyName;
            """;

        var query = await connection.QueryAsync<GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearDto>(sql: sql, param: new
        {
            academicYearName
        });

        var data = query.AsList();
        var result = new ReportResponseWrapper<AcademicYearReportResponseWrapper<PercentageTotalContributionsPerFacultyPerAcademicYearReportDto>>();

        var academicYearDto = new AcademicYearReportResponseWrapper<PercentageTotalContributionsPerFacultyPerAcademicYearReportDto>();

        if (data.Count > 0)
        {
            academicYearDto.AcademicYear = data[0].AcademicYear;

            for (var i = 0; i < data.Count; i++)
            {
                var percentage = new PercentageTotalContributionsPerFacultyPerAcademicYearReportDto();

                percentage.Faculty = data[i].Faculty;
                percentage.Percentage = data[i].Percentage;

                academicYearDto.DataSets.Add(percentage);
            }

            result.Response.Add(academicYearDto);
        }

        return result;
    }
}
