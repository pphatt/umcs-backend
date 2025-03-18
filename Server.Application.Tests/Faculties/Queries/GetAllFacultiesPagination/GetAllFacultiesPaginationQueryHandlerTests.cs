using FluentAssertions;

using Moq;

using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Features.FacultyApp.Queries.GetAllFacultiesPagination;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Tests.Faculties.Queries.GetAllFacultiesPagination;

using Faculty = Server.Domain.Entity.Content.Faculty;

[Trait("Faculty", "Get All Faculties Pagination")]
public class GetAllFacultiesPaginationQueryHandlerTests : BaseTest
{
    private readonly GetAllFacultiesPaginationQueryHandler _queryHandler;
    private readonly List<Faculty> _faculties;

    public GetAllFacultiesPaginationQueryHandlerTests()
    {
        _faculties = new List<Faculty>
        {
            new Faculty
            {
                Id = Guid.NewGuid(),
                Name = "Engineering",
                DateCreated = DateTime.UtcNow.AddYears(-2),
                DateDeleted = null
            },
            new Faculty
            {
                Id = Guid.NewGuid(),
                Name = "Science",
                DateCreated = DateTime.UtcNow.AddYears(-1),
                DateDeleted = null
            },
            new Faculty
            {
                Id = Guid.NewGuid(),
                Name = "Arts",
                DateCreated = DateTime.UtcNow,
                DateDeleted = null
            },
            new Faculty
            {
                Id = Guid.NewGuid(),
                Name = "Business",
                DateCreated = DateTime.UtcNow.AddMonths(-6),
                DateDeleted = DateTime.UtcNow
            }
        };

        _queryHandler = new GetAllFacultiesPaginationQueryHandler(_mockUnitOfWork.Object, _mapper);
    }

    [Fact]
    public async Task GetAllFacultiesPaginationQueryHandler_GetAllFacultiesPagination_WithNoQueryParams_ShouldReturnAllActiveFacultiesPaginated()
    {
        // Arrange
        var query = new GetAllFacultiesPaginationQuery
        {
        };

        var activeFaculties = _faculties.Where(f => f.DateDeleted == null).ToList();

        var skipPage = (query.PageIndex - 1) * query.PageSize;

        var expectedResult = new PaginationResult<FacultyDto>
        {
            CurrentPage = query.PageIndex,
            PageSize = query.PageSize,
            RowCount = activeFaculties.Count,
            Results = activeFaculties
                .OrderByDescending(x => x.DateCreated)
                .Skip(skipPage)
                .Take(query.PageSize)
                .Select(x => new FacultyDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DateCreated = x.DateCreated
                }).ToList()
        };

        _mockFacultyRepository
            .Setup(repo => repo.GetAllFacultiesPagination(null, query.PageIndex, query.PageSize))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper<PaginationResult<FacultyDto>>>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.ResponseData.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetAllFacultiesPaginationQueryHandler_GetAllFacultiesPagination_WithKeyword_ShouldReturnFilteredFacultiesPaginated()
    {
        // Arrange
        var query = new GetAllFacultiesPaginationQuery
        {
            Keyword = "Sci"
        };

        var filteredFaculties = _faculties
            .Where(x => x.Name.Contains(query.Keyword) && x.DateDeleted == null)
            .ToList();

        var skipPage = (query.PageIndex - 1) * query.PageSize;

        var expectedResult = new PaginationResult<FacultyDto>
        {
            CurrentPage = query.PageIndex,
            PageSize = query.PageSize,
            RowCount = filteredFaculties.Count,
            Results = filteredFaculties
                .OrderByDescending(x => x.DateCreated)
                .Skip(skipPage)
                .Take(query.PageSize)
                .Select(x => new FacultyDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DateCreated = x.DateCreated
                }).ToList()
        };

        _mockFacultyRepository
            .Setup(repo => repo.GetAllFacultiesPagination(query.Keyword, query.PageIndex, query.PageSize))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.ResponseData.RowCount.Should().Be(1);
        result.Value.ResponseData.Results.Should().HaveCount(1);
        result.Value.ResponseData.Results.First().Name.Should().Contain("Science");
    }

    [Fact]
    public async Task GetAllFacultiesPaginationQueryHandler_GetAllFacultiesPagination_WithDifferentPageIndex_ShouldReturnCorrectPage()
    {
        // Arrange
        var query = new GetAllFacultiesPaginationQuery
        {
            PageIndex = 2,
            PageSize = 2,
            Keyword = null
        };

        var activeFaculties = _faculties.Where(f => f.DateDeleted == null).ToList();

        var skipPage = (query.PageIndex - 1) * query.PageSize;

        var expectedResult = new PaginationResult<FacultyDto>
        {
            CurrentPage = query.PageIndex,
            PageSize = query.PageSize,
            RowCount = activeFaculties.Count,
            Results = activeFaculties
                .OrderByDescending(x => x.DateCreated)
                .Skip(skipPage)
                .Take(query.PageSize)
                .Select(x => new FacultyDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DateCreated = x.DateCreated
                }).ToList()
        };

        _mockFacultyRepository
            .Setup(repo => repo.GetAllFacultiesPagination(null, query.PageIndex, query.PageSize))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.ResponseData.CurrentPage.Should().Be(2);
        result.Value.ResponseData.Results.Should().HaveCount(1);
        result.Value.ResponseData.Results.First().Name.Should().Be("Engineering");
    }

    [Fact]
    public async Task GetAllFacultiesPaginationQueryHandler_GetAllFacultiesPagination_WithEmptyResult_ShouldReturnEmptyPagination()
    {
        // Arrange
        var query = new GetAllFacultiesPaginationQuery
        {
            PageIndex = 1,
            PageSize = 10,
            Keyword = "nonexistent"
        };

        var expectedResult = new PaginationResult<FacultyDto>
        {
            CurrentPage = query.PageIndex,
            PageSize = query.PageSize,
            RowCount = 0,
            Results = new List<FacultyDto>()
        };

        _mockFacultyRepository
            .Setup(repo => repo.GetAllFacultiesPagination("nonexistent", query.PageIndex, query.PageSize))
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
