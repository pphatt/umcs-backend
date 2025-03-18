using FluentAssertions;

using Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;
using Server.Contracts.AcademicYears.CreateAcademicYear;

namespace Server.Application.Tests.AcademicYears.Commands.CreateAcademicYear;

[Trait("Academic Year", "Create")]
public class CreateAcademicYearCommandTests : BaseTest
{
    [Fact]
    public void CreateAcademicYearCommand_CreateAcademicYear_MapCorrectly()
    {
        // Arrange
        var request = new CreateAcademicYearRequest
        {
            Name = "2025-2026",
            IsActive = true,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
        };

        // Act
        var command = _mapper.Map<CreateAcademicYearCommand>(request);

        // Assert
        command.Should().NotBeNull();
        command.Name.Should().Be(request.Name);
        command.IsActive.Should().Be(request.IsActive);
        command.StartClosureDate.Should().Be(request.StartClosureDate);
        command.EndClosureDate.Should().Be(request.EndClosureDate);
        command.FinalClosureDate.Should().Be(request.FinalClosureDate);
    }
}
