using FluentValidation.TestHelper;

using Server.Application.Features.AcademicYearsApp.Commands.DeleteAcademicYear;

namespace Server.Application.Tests.AcademicYears.Commands.DeleteAcademicYear;

[Trait("Academic Year", "Delete")]
public class DeleteAcademicYearCommandValidatorTests : BaseTest
{
    private readonly DeleteAcademicYearCommandValidator _validator;

    public DeleteAcademicYearCommandValidatorTests()
    {
        _validator = new DeleteAcademicYearCommandValidator();
    }

    [Fact]
    public async Task DeleteAcademicYearCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new DeleteAcademicYearCommand
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);


        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
