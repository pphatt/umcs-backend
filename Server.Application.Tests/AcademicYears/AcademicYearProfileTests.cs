using FluentAssertions;

using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Domain.Entity.Content;

namespace Server.Application.Tests.AcademicYears;

public class AcademicYearProfileTests : BaseTest
{
    [Fact]
    public async Task CreateMap_FromAcademicYearToAcademicYearDto_MapsCorrectly()
    {
        // Arrange
        var academicYear = new AcademicYear
        {
            Name = "2025-2026",
            IsActive = true,
            UserIdCreated = Guid.Parse("613DA9F6-FC5A-4E7F-AB2E-7FC89258A596"),
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
        };

        // Act
        var academicYearDto = _mapper.Map<AcademicYearDto>(academicYear);

        // Assert
        academicYearDto.Should().NotBeNull();
        academicYearDto.Name.Should().Be(academicYear.Name);
        academicYearDto.IsActive.Should().Be(academicYear.IsActive);
        academicYearDto.StartClosureDate.Should().Be(academicYear.StartClosureDate);
        academicYearDto.EndClosureDate.Should().Be(academicYear.EndClosureDate);
        academicYearDto.FinalClosureDate.Should().Be(academicYear.FinalClosureDate);
    }
}
