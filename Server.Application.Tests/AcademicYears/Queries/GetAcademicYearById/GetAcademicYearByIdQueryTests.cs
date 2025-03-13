using FluentAssertions;

using Server.Application.Features.AcademicYearsApp.Queries.GetAcademicYearById;
using Server.Contracts.AcademicYears.GetAcademicYearById;

namespace Server.Application.Tests.AcademicYears.Queries.GetAcademicYearById;

[Trait("Academic Year", "Get Academic Year By Id")]
public class GetAcademicYearByIdQueryTests : BaseTest
{
    [Fact]
    public void GetAcademicYearByIdQuery_GetAcademicYearById_MapCorrectly()
    {
        // Arrange
        var request = new GetAcademicYearByIdRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = _mapper.Map<GetAcademicYearByIdQuery>(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(request.Id);
    }
}
