using FluentValidation.TestHelper;

using Server.Application.Features.AcademicYearsApp.Commands.InactivateAcademicYear;

namespace Server.Application.Tests.AcademicYears.Commands.InactivateAcademicYear;

[Trait("Academic Year", "Inactivate")]
public class InactivateAcademicYearCommandValidatorTests : BaseTest
{
    private readonly InactivateAcademicYearCommandValidator _validator;

    public InactivateAcademicYearCommandValidatorTests()
    {
        _validator = new InactivateAcademicYearCommandValidator();
    }

    [Fact]
    public async Task InactivateAcademicYearCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new InactivateAcademicYearCommand
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);


        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
