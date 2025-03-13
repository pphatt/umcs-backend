using FluentAssertions;

using Server.Application.Features.FacultyApp.Queries.GetFacultyById;
using Server.Contracts.Faculties.GetFacultyById;

namespace Server.Application.Tests.Faculties.Queries.GetFacultyById;

[Trait("Faculty", "Get Faculty By Id")]
public class GetFacultyByIdQueryTests : BaseTest
{
    [Fact]
    public void GetFacultyByIdQuery_GetFacultyById_MapCorrectly()
    {
        // Arrange
        var request = new GetFacultyByIdRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = _mapper.Map<GetFacultyByIdQuery>(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(request.Id);
    }
}
