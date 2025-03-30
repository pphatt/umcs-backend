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

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributionsInEachFacultyForAnyAcademicYearReportDto>>> GetTotalContributionsInEachFacultyForAnyAcademicYear(string academicYearName)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql =
            """
            SELECT ay.Name AS AcademicYear,
                   f.Name as Faculty,
                   COALESCE(COUNT(c.Id), 0) AS TotalContributions
            FROM AcademicYears ay
            CROSS JOIN Faculties f
            LEFT JOIN Contributions c ON c.AcademicYearId = ay.Id AND c.FacultyId = f.Id
            WHERE f.DateDeleted is null AND ay.Id = (SELECT Id FROM AcademicYears WHERE Name = '2025-2026')
            GROUP BY ay.Name, f.Name
            ORDER BY ay.Name, f.Name;
            """;

        var query = await connection.QueryAsync<GetTotalContributionsInEachFacultyForAnyAcademicYearDto>(sql: sql, param: new
        {
            academicYearName
        });

        var data = query.AsList();
        var result = new ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributionsInEachFacultyForAnyAcademicYearReportDto>>();

        var academicYearDto = new AcademicYearReportResponseWrapper<GetTotalContributionsInEachFacultyForAnyAcademicYearReportDto>();

        if (data.Count > 0)
        {
            academicYearDto.AcademicYear = data[0].AcademicYear;

            for (var i = 0; i < data.Count; i++)
            {
                var count = new GetTotalContributionsInEachFacultyForAnyAcademicYearReportDto();

                count.Faculty = data[i].Faculty;
                count.TotalContributions = data[i].TotalContributions;

                academicYearDto.DataSets.Add(count);
            }

            result.Response.Add(academicYearDto);
        }

        return result;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearReportDto>>> GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYear()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql =
            """
            SELECT
                ay.Name AS AcademicYear,
                cc.Faculty AS Faculty,
                COALESCE(CAST((cc.ContributionCount * 1.0 / tc.TotalCount) * 100 AS INT), 0) AS Percentage
            FROM AcademicYears ay
            CROSS JOIN (
                SELECT
                    ay.Name AS AcademicYear,
                    f.Name AS Faculty,
                    COUNT(c.Id) AS ContributionCount
                FROM AcademicYears ay
                CROSS JOIN Faculties f
                LEFT JOIN Contributions c ON f.Id = c.FacultyId
                    AND c.AcademicYearId = ay.Id
                    AND c.DateDeleted IS NULL
                WHERE f.DateDeleted IS NULL
                GROUP BY ay.Name, f.Name
            ) AS cc
            LEFT JOIN (
                SELECT
                    ay.Name AS AcademicYear,
                    COUNT(*) AS TotalCount
                FROM Contributions c
                JOIN AcademicYears ay ON c.AcademicYearId = ay.Id
                JOIN Faculties f ON c.FacultyId = f.Id
                WHERE c.DateDeleted IS NULL AND f.DateDeleted IS NULL
                GROUP BY ay.Name
            ) AS tc ON ay.Name = tc.AcademicYear
            WHERE ay.Name = cc.AcademicYear
            ORDER BY ay.Name, cc.Faculty;
            """;

        var query = await connection.QueryAsync<GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearDto>(sql: sql);

        var data = query.AsList();

        var count = query.Count();
        var facultyCount = await _facultyRepository.CountAsync();

        var result = new ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearReportDto>>();

        for (var i = 0; i < count; i += facultyCount)
        {
            var academicYearResult = new AcademicYearReportResponseWrapper<GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearReportDto>();

            academicYearResult.AcademicYear = data[i].AcademicYear;

            for (var j = i; j < i + facultyCount; j++)
            {
                var facultyResult = new GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearReportDto();

                facultyResult.Faculty = data[j].Faculty;
                facultyResult.Percentage = data[j].Percentage;

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

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsInEachFacultyInEachAcademicYearReportDto>>> GetTotalContributorsInEachFacultyInEachAcademicYear()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql =
            """
            SELECT ay.Name as AcademicYear,
            f.Name as Faculty,
            COALESCE(COUNT(distinct c.UserId), 0) AS Contributors
            FROM AcademicYears ay
            CROSS JOIN Faculties f
            LEFT JOIN Contributions c ON c.AcademicYearId = ay.Id AND c.FacultyId = f.Id
            WHERE f.DateDeleted is null
            GROUP BY ay.Name, f.Name
            ORDER BY ay.Name, f.Name;
            """;

        var query = await connection.QueryAsync<GetTotalContributorsInEachFacultyInEachAcademicYearDto>(sql: sql);

        var result = new ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsInEachFacultyInEachAcademicYearReportDto>>();

        var data = query.AsList();

        var count = data.Count;
        var facultyCount = await _facultyRepository.CountAsync();

        for (var i = 0; i < count; i += facultyCount)
        {
            var academicYearResult = new AcademicYearReportResponseWrapper<GetTotalContributorsInEachFacultyInEachAcademicYearReportDto>();

            academicYearResult.AcademicYear = data[i].AcademicYear;

            for (var j = i; j < i + facultyCount; j++)
            {
                var facultyResult = new GetTotalContributorsInEachFacultyInEachAcademicYearReportDto();

                facultyResult.Faculty = data[j].Faculty;
                facultyResult.Contributors = data[j].Contributors;

                academicYearResult.DataSets.Add(facultyResult);
            }

            result.Response.Add(academicYearResult);
        }

        result.Response = result.Response
            .OrderByDescending(x => x.AcademicYear)
            .ToList();

        return result;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsByEachFacultyForAnyAcademicYearReportDto>>> GetTotalContributorsByEachFacultyForAnyAcademicYear(string academicYearName)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql =
            """
            SELECT ay.Name as AcademicYear,
                   f.Name as Faculty,
                   COALESCE(COUNT(distinct c.UserId), 0) AS Contributors
            FROM AcademicYears ay
            CROSS JOIN Faculties f
            LEFT JOIN Contributions c ON c.AcademicYearId = ay.Id AND c.FacultyId = f.Id
            WHERE f.DateDeleted is null AND ay.Name = @academicYearName
            GROUP BY ay.Name, f.Name
            ORDER BY ay.Name, f.Name;
            """;

        var query = await connection.QueryAsync<GetTotalContributorsByEachFacultyForAnyAcademicYearDto>(sql: sql, param: new
        {
            academicYearName,
        });

        var data = query.AsList();

        var result = new ReportResponseWrapper<AcademicYearReportResponseWrapper<GetTotalContributorsByEachFacultyForAnyAcademicYearReportDto>>();

        var academicYearDto = new AcademicYearReportResponseWrapper<GetTotalContributorsByEachFacultyForAnyAcademicYearReportDto>();

        if (data.Count > 0)
        {
            academicYearDto.AcademicYear = data[0].AcademicYear;

            for (var i = 0; i < data.Count; i++)
            {
                var percentage = new GetTotalContributorsByEachFacultyForAnyAcademicYearReportDto();

                percentage.Faculty = data[i].Faculty;
                percentage.Contributors = data[i].Contributors;

                academicYearDto.DataSets.Add(percentage);
            }

            result.Response.Add(academicYearDto);
        }

        return result;
    }

    public async Task<ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearReportDto>>> GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearReport()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();

        var sql =
            """
            SELECT
                ay.Name AS AcademicYear,
                cc.Faculty AS Faculty,
                COALESCE(CAST((cc.ContributionCount * 1.0 / tc.TotalCount) * 100 AS INT), 0) AS Percentage
            FROM AcademicYears ay
            CROSS JOIN (
                SELECT
                    ay.Name AS AcademicYear,
                    f.Name AS Faculty,
                    COUNT(distinct c.UserId) AS ContributionCount
                FROM AcademicYears ay
                CROSS JOIN Faculties f
                LEFT JOIN Contributions c ON f.Id = c.FacultyId
                    AND c.AcademicYearId = ay.Id
                    AND c.DateDeleted IS NULL
                WHERE f.DateDeleted IS NULL
                GROUP BY ay.Name, f.Name
            ) AS cc
            LEFT JOIN (
                SELECT
                    ay.Name AS AcademicYear,
                    COUNT(distinct c.UserId) AS TotalCount
                FROM Contributions c
                JOIN AcademicYears ay ON c.AcademicYearId = ay.Id
                JOIN Faculties f ON c.FacultyId = f.Id
                WHERE c.DateDeleted IS NULL AND f.DateDeleted IS NULL
                GROUP BY ay.Name
            ) AS tc ON ay.Name = tc.AcademicYear
            WHERE ay.Name = cc.AcademicYear
            ORDER BY ay.Name, cc.Faculty;
            """;

        var query = await connection.QueryAsync<GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearDto>(sql: sql);

        var result = new ReportResponseWrapper<AcademicYearReportResponseWrapper<GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearReportDto>>();

        var data = query.AsList();

        var count = data.Count;
        var facultyCount = await _facultyRepository.CountAsync();

        for (var i = 0; i < count; i += facultyCount)
        {
            var academicYearResult = new AcademicYearReportResponseWrapper<GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearReportDto>();

            academicYearResult.AcademicYear = data[i].AcademicYear;

            for (var j = i; j < i + facultyCount; j++)
            {
                var facultyResult = new GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearReportDto();

                facultyResult.Faculty = data[j].Faculty;
                facultyResult.Percentage = data[j].Percentage;

                academicYearResult.DataSets.Add(facultyResult);
            }

            result.Response.Add(academicYearResult);
        }

        result.Response = result.Response
            .OrderByDescending(x => x.AcademicYear)
            .ToList();

        return result;
    }
}
