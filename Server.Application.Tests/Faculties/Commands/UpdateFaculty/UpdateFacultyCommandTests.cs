using FluentAssertions;

using Server.Application.Features.FacultyApp.Commands.UpdateFaculty;
using Server.Contracts.Faculties.UpdateFaculty;

namespace Server.Application.Tests.Faculties.Commands.UpdateFaculty;

[Trait("Faculty", "Update")]
public class UpdateFacultyCommandTests : BaseTest
{
    [Fact]
    public void UpdateFacultyCommand_UpdateFaculty_MapCorrectly()
    {
        // Arrange
        var request = new UpdateFacultyRequest
        {
            Id = Guid.NewGuid(),
            Name = "IT"
        };

        // Act
        var result = _mapper.Map<UpdateFacultyCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(request.Id);
        result.Name.Should().Be(request.Name);
    }
}
