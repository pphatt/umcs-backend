using FluentValidation.TestHelper;

using Server.Application.Features.AcademicYearsApp.Commands.BulkDeleteAcademicYears;

namespace Server.Application.Tests.AcademicYears.Commands.BulkDeleteAcademicYears;

[Trait("Academic Year", "Bulk Delete")]
public class BulkDeleteAcademicYearsCommandValidatorTests : BaseTest
{
    private readonly BulkDeleteAcademicYearsCommandValidator _validator;

    public BulkDeleteAcademicYearsCommandValidatorTests()
    {
        _validator = new BulkDeleteAcademicYearsCommandValidator();
    }

    [Fact]
    public async Task BulkDeleteAcademicYearsCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new BulkDeleteAcademicYearsCommand
        {
            AcademicIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _validator.TestValidateAsync(command);


        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
