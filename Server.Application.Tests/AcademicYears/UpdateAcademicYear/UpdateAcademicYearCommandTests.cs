using FluentAssertions;

using Server.Application.Features.AcademicYearsApp.Commands.UpdateAcademicYear;
using Server.Contracts.AcademicYears.UpdateAcademicYear;

namespace Server.Application.Tests.AcademicYears.UpdateAcademicYear;

[Trait("Academic Year", "Update")]
public class UpdateAcademicYearCommandTests : BaseTest
{
    [Fact]
    public void UpdateAcademicYearCommand_UpdateAcademicYear_MapCorrectly()
    {
        // Arrange
        var request = new UpdateAcademicYearRequest
        {
            Id = Guid.NewGuid(),
            AcademicYearName = "2025-2026",
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2)
        };

        // Act
        var command = _mapper.Map<UpdateAcademicYearCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.Id.Should().Be(request.Id);
        command.AcademicYearName.Should().Be(request.AcademicYearName);
        command.StartClosureDate.Should().Be(request.StartClosureDate);
        command.EndClosureDate.Should().Be(request.EndClosureDate);
        command.FinalClosureDate.Should().Be(request.FinalClosureDate);
    }
}
