using FluentAssertions;

using Server.Application.Features.AcademicYearsApp.Commands.BulkDeleteAcademicYears;
using Server.Contracts.AcademicYears.BulkDeleteAcademicYears;

namespace Server.Application.Tests.AcademicYears.Commands.BulkDeleteAcademicYears;

[Trait("Academic Year", "Bulk Delete")]
public class BulkDeleteAcademicYearsCommandTests : BaseTest
{
    [Fact]
    public void BulkDeleteAcademicYearsCommand_BulkDeleteAcademicYears_MapCorrectly()
    {
        // Arrange
        var academicIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var request = new BulkDeleteAcademicYearsRequest
        {
            AcademicIds = academicIds
        };

        // Act
        var result = _mapper.Map<BulkDeleteAcademicYearsCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.AcademicIds.Should().NotBeNull();
        result.AcademicIds.Should().BeEquivalentTo(academicIds, "AcademicIds should match the request exactly");
    }
}
