using FluentValidation.TestHelper;

using Server.Application.Features.AcademicYearsApp.Commands.ActivateAcademicYear;

namespace Server.Application.Tests.AcademicYears.Commands.ActivateAcademicYear;

[Trait("Academic Year", "Activate")]
public class ActivateAcademicYearCommandValidatorTests : BaseTest
{
    private readonly ActivateAcademicYearCommandValidator _validator;

    public ActivateAcademicYearCommandValidatorTests()
    {
        _validator = new ActivateAcademicYearCommandValidator();
    }

    [Fact]
    public async Task ActivateAcademicYearCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new ActivateAcademicYearCommand
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
