using System.Threading.Tasks;

using FluentValidation.TestHelper;

using Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;
using Server.Application.Features.FacultyApp.Commands.DeleteFaculty;

namespace Server.Application.Tests.Faculties.Commands.DeleteFaculty;

[Trait("Faculty", "Delete")]
public class DeleteFacultyCommandValidatorTests : BaseTest
{
    private readonly DeleteFacultyCommandValidator _validator;

    public DeleteFacultyCommandValidatorTests()
    {
        _validator = new DeleteFacultyCommandValidator();
    }

    [Fact]
    public async Task DeleteFacultyCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new DeleteFacultyCommand
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task DeleteFacultyCommandValidator_DeleteFaculty_Should_ReturnError_WhenFacultyIsNullOrEmpty()
    {
        // Arrange
        var command = new DeleteFacultyCommand
        {
            Id = Guid.Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id);
    }
}
