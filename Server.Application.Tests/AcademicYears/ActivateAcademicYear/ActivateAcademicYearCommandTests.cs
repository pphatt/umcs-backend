using FluentAssertions;

using Server.Application.Features.AcademicYearsApp.Commands.ActivateAcademicYear;
using Server.Contracts.AcademicYears.ActivateAcademicYear;

namespace Server.Application.Tests.AcademicYears.ActivateAcademicYear;

[Trait("Academic Year", "Activate")]
public class ActivateAcademicYearCommandTests : BaseTest
{
    [Fact]
    public void ActivateAcademicYearCommand_ActivateAcademicYear_MapCorrectly()
    {
        // Arrange
        var request = new ActivateAcademicYearRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = _mapper.Map<ActivateAcademicYearCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(request.Id);
    }
}
