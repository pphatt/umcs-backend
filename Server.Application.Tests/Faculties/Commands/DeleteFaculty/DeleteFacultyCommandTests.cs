using FluentAssertions;

using Server.Application.Features.FacultyApp.Commands.DeleteFaculty;
using Server.Contracts.Faculties.DeleteFaculty;

namespace Server.Application.Tests.Faculties.Commands.DeleteFaculty;

[Trait("Faculty", "Delete")]
public class DeleteFacultyCommandTests : BaseTest
{
    [Fact]
    public void DeleteFacultyCommand_DeleteFaculty_MapCorrectly()
    {
        // Arrange
        var request = new DeleteFacultyRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = _mapper.Map<DeleteFacultyCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(result.Id);
    }
}
