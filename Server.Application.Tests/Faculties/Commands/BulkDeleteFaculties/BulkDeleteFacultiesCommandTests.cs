using FluentAssertions;

using Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculty;
using Server.Contracts.Faculties.BulkDeleteFaculties;

namespace Server.Application.Tests.Faculties.Commands.BulkDeleteFaculties;

[Trait("Faculty", "Bulk Delete")]
public class BulkDeleteFacultiesCommandTests : BaseTest
{
    [Fact]
    public void BulkDeleteFacultiesCommand_BulkDeleteFaculties_MapCorrectly()
    {
        // Arrange
        var request = new BulkDeleteFacultiesRequest
        {
            FacultyIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = _mapper.Map<BulkDeleteFacultiesCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.FacultyIds.Should().BeEquivalentTo(result.FacultyIds);
    }
}
