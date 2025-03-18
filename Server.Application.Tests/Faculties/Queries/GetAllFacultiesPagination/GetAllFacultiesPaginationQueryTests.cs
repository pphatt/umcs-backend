using FluentAssertions;

using Server.Application.Features.FacultyApp.Queries.GetAllFacultiesPagination;
using Server.Contracts.Faculties.GetAllFacultiesPagination;

namespace Server.Application.Tests.Faculties.Queries.GetAllFacultiesPagination;

[Trait("Faculty", "Get All Faculties Pagination")]
public class GetAllFacultiesPaginationQueryTests : BaseTest
{
    [Theory]
    [InlineData(null, 1, 10)]
    [InlineData("", 1, 2)]
    [InlineData("abc", 2, 10)]
    public void GetAllFacultiesPaginationQuery_GetAllFacultiesPagination_MapCorrectly(string? keyword, int pageIndex, int pageSize)
    {
        // Arrange
        var request = new GetAllFacultiesPaginationRequest
        {
            Keyword = keyword,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        // Act
        var result = _mapper.Map<GetAllFacultiesPaginationQuery>(request);

        // Assert
        result.Should().NotBeNull();
        result.Keyword.Should().Be(request.Keyword);
        result.PageIndex.Should().Be(request.PageIndex);
        result.PageSize.Should().Be(request.PageSize);
    }
}
