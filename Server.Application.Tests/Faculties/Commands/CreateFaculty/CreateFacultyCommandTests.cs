using FluentAssertions;

using Server.Application.Features.FacultyApp.Commands.CreateFaculty;
using Server.Contracts.Faculties.CreateFaculty;

namespace Server.Application.Tests.Faculties.Commands.CreateFaculty;

[Trait("Faculty", "Create")]
public class CreateFacultyCommandTests : BaseTest
{
    [Fact]
    public void CreateFacultyCommand_CreateFaculty_MapCorrectly()
    {
        // Arrange
        var request = new CreateFacultyRequest
        {
            Name = "IT"
        };

        // Act
        var result = _mapper.Map<CreateFacultyCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
    }
}
