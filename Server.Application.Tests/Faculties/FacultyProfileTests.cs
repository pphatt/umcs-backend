using FluentAssertions;

using Server.Application.Common.Dtos.Content.Faculty;
using Server.Domain.Entity.Content;

namespace Server.Application.Tests.Faculties;

public class FacultyProfileTests : BaseTest
{
    [Fact]
    public async Task CreateMap_FromFacultyToFacultyDto_MapCorrectly()
    {
        // Arrange
        var faculty = new Faculty
        {
            Id = Guid.NewGuid(),
            Name = "IT",
        };

        // Act
        var facultyDto = _mapper.Map<FacultyDto>(faculty);

        // Assert
        facultyDto.Should().NotBeNull();
        facultyDto.Id.Should().Be(faculty.Id);
        facultyDto.Name.Should().Be(faculty.Name);
    }
}
