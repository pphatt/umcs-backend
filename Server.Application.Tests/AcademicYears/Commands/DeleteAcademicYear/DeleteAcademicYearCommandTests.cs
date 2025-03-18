using FluentAssertions;

using Server.Application.Features.AcademicYearsApp.Commands.DeleteAcademicYear;
using Server.Contracts.AcademicYears.DeleteAcademicYear;

namespace Server.Application.Tests.AcademicYears.Commands.DeleteAcademicYear;

[Trait("Academic Year", "Delete")]
public class DeleteAcademicYearCommandTests : BaseTest
{
    [Fact]
    public void DeleteAcademicYearCommand_DeleteAcademicYear_MapCorrectly()
    {
        // Arrange
        var request = new DeleteAcademicYearRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        var command = _mapper.Map<DeleteAcademicYearCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.Id.Should().Be(request.Id);
    }
}
