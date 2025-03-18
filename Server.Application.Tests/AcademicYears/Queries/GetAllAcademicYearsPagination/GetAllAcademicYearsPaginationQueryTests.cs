using FluentAssertions;

using Server.Application.Features.AcademicYearsApp.Queries.GetAllAcademicYearsPagination;
using Server.Contracts.AcademicYears.GetAllAcademicYearsPagination;

namespace Server.Application.Tests.AcademicYears.Queries.GetAllAcademicYearsPagination;


[Trait("Academic Year", "Get All Academic Years Pagination")]
public class GetAllAcademicYearsPaginationQueryTests : BaseTest
{
    [Theory]
    [InlineData(null, 1, 10)]
    [InlineData("", 1, 2)]
    [InlineData("abc", 2, 10)]
    public void GetAllAcademicYearsPaginationQuery_GetAllAcademicYearsPagination_MapCorrectly(string? keyword, int pageIndex, int pageSize)
    {
        // Arrange
        var request = new GetAllAcademicYearsPaginationRequest
        {
            Keyword = keyword,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        // Act
        var result = _mapper.Map<GetAllAcademicYearsPaginationQuery>(request);

        // Assert
        result.Should().NotBeNull();
        result.Keyword.Should().Be(request.Keyword);
        result.PageIndex.Should().Be(request.PageIndex);
        result.PageSize.Should().Be(request.PageSize);
    }
}
