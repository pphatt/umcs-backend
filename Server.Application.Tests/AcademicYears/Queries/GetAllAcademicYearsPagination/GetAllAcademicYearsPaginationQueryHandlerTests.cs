using FluentAssertions;

using Moq;

using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Features.AcademicYearsApp.Queries.GetAllAcademicYearsPagination;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Tests.AcademicYears.Queries.GetAllAcademicYearsPagination;

[Trait("Academic Year", "Get All Academic Years Pagination")]
public class GetAllAcademicYearsPaginationQueryHandlerTests : BaseTest
{
    private readonly GetAllAcademicYearsPaginationQueryHandler _queryHandler;
    private readonly List<AcademicYear> _academicYears;

    public GetAllAcademicYearsPaginationQueryHandlerTests()
    {
        _academicYears = new List<AcademicYear>
        {
            new AcademicYear
            {
                Id = Guid.NewGuid(),
                Name = "2023-2024",
                UserIdCreated = Guid.NewGuid(),
                IsActive = true,
                DateCreated = DateTime.UtcNow.AddYears(-2),
                StartClosureDate = DateTime.UtcNow.AddYears(-2),
                EndClosureDate = DateTime.UtcNow.AddYears(-2).AddMonths(1),
                FinalClosureDate = DateTime.UtcNow.AddYears(-2).AddMonths(2)
            },
            new AcademicYear
            {
                Id = Guid.NewGuid(),
                Name = "2024-2025",
                UserIdCreated = Guid.NewGuid(),
                IsActive = true,
                DateCreated = DateTime.UtcNow.AddYears(-1),
                StartClosureDate = DateTime.UtcNow.AddYears(-1),
                EndClosureDate = DateTime.UtcNow.AddYears(-1).AddMonths(1),
                FinalClosureDate = DateTime.UtcNow.AddYears(-1).AddMonths(2)
            },
            new AcademicYear
            {
                Id = Guid.NewGuid(),
                Name = "2025-2026",
                UserIdCreated = Guid.NewGuid(),
                IsActive = true,
                DateCreated = DateTime.UtcNow,
                StartClosureDate = DateTime.UtcNow,
                EndClosureDate = DateTime.UtcNow.AddMonths(1),
                FinalClosureDate = DateTime.UtcNow.AddMonths(2)
            }
        };

        _queryHandler = new GetAllAcademicYearsPaginationQueryHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAllAcademicYearsPaginationQueryHandler_GetAllAcademicYearsPaginationQuery_WithNoQueryParams_ShouldReturnAllActiveAcademicYearsPaginated()
    {
        // Arrange
        var query = new GetAllAcademicYearsPaginationQuery
        {
        };

        var expectedResult = new PaginationResult<AcademicYearDto>
        {
            CurrentPage = 1,
            PageSize = 10,
            RowCount = 3,
            Results = _academicYears
                .OrderByDescending(x => x.DateCreated)
                .Take(10)
                .Select(x => new AcademicYearDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    StartClosureDate = x.StartClosureDate,
                    EndClosureDate = x.EndClosureDate,
                    FinalClosureDate = x.FinalClosureDate
                }).ToList()
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.GetAllAcademicYearsPagination(null, 1, 10))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper<PaginationResult<AcademicYearDto>>>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.ResponseData.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetAllAcademicYearsPaginationQueryHandler_GetAllAcademicYearsPaginationQuery_WithKeyword_ShouldReturnFilteredAcademicYearsPaginated()
    {
        // Arrange
        var query = new GetAllAcademicYearsPaginationQuery
        {
            Keyword = "2025"
        };

        var filteredAcademicYears = _academicYears
            .Where(x => x.Name.Contains("2025"))
            .ToList();

        var expectedResult = new PaginationResult<AcademicYearDto>
        {
            CurrentPage = 1,
            PageSize = 10,
            RowCount = 2,
            Results = filteredAcademicYears
                .OrderByDescending(x => x.DateCreated)
                .Take(2)
                .Select(x => new AcademicYearDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    StartClosureDate = x.StartClosureDate,
                    EndClosureDate = x.EndClosureDate,
                    FinalClosureDate = x.FinalClosureDate
                }).ToList()
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.GetAllAcademicYearsPagination("2025", 1, 10))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.ResponseData.RowCount.Should().Be(2);
        result.Value.ResponseData.Results.Should().HaveCount(2);
        result.Value.ResponseData.Results.First().Name.Should().Contain("2025");
    }

    [Fact]
    public async Task GetAllAcademicYearsPaginationQueryHandler_GetAllAcademicYearsPaginationQuery_WithDifferentPageIndex_ShouldReturnCorrectPage()
    {
        // Arrange
        var query = new GetAllAcademicYearsPaginationQuery
        {
            PageIndex = 2,
            PageSize = 2,
            Keyword = null
        };

        var expectedResult = new PaginationResult<AcademicYearDto>
        {
            CurrentPage = 2,
            PageSize = 2,
            RowCount = 3,
            Results = _academicYears
                .OrderByDescending(x => x.DateCreated)
                .Skip(2)
                .Take(2)
                .Select(x => new AcademicYearDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    StartClosureDate = x.StartClosureDate,
                    EndClosureDate = x.EndClosureDate,
                    FinalClosureDate = x.FinalClosureDate
                }).ToList()
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.GetAllAcademicYearsPagination(null, 2, 2))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.ResponseData.CurrentPage.Should().Be(2);
        result.Value.ResponseData.Results.Should().HaveCount(1); // Only 1 item left after skipping 2
    }

    [Fact]
    public async Task GetAllAcademicYearsPaginationQueryHandler_GetAllAcademicYearsPaginationQuery_WithEmptyResult_ShouldReturnEmptyPagination()
    {
        // Arrange
        var query = new GetAllAcademicYearsPaginationQuery
        {
            PageIndex = 1,
            PageSize = 10,
            Keyword = "nonexistent"
        };

        var expectedResult = new PaginationResult<AcademicYearDto>
        {
            CurrentPage = 1,
            PageSize = 10,
            RowCount = 0,
            Results = new List<AcademicYearDto>()
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.GetAllAcademicYearsPagination("nonexistent", 1, 10))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.ResponseData.RowCount.Should().Be(0);
        result.Value.ResponseData.Results.Should().BeEmpty();
    }
}
