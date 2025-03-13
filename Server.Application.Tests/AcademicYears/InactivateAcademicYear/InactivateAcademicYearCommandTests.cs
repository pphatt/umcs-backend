using FluentAssertions;

using Server.Application.Features.AcademicYearsApp.Commands.InactivateAcademicYear;
using Server.Contracts.AcademicYears.InactivateAcademicYear;

namespace Server.Application.Tests.AcademicYears.InactivateAcademicYear;

[Trait("Academic Year", "Inactivate")]
public class InactivateAcademicYearCommandTests : BaseTest
{
    [Fact]
    public void InactiveAcademicYearCommand_InactiveAcademicYear_MapCorrect()
    {
        // Arrange
        var request = new InactivateAcademicYearRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = _mapper.Map<InactivateAcademicYearCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(request.Id);
    }
}
