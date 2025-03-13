using FluentValidation.TestHelper;

using Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculties;
using Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculty;

namespace Server.Application.Tests.Faculties.Commands.BulkDeleteFaculties;

[Trait("Faculty", "Bulk Delete")]
public class BulkDeleteFacultiesCommandValidatorTests
{
    private readonly BulkDeleteFacultiesCommandValidator _validator;

    public BulkDeleteFacultiesCommandValidatorTests()
    {
        _validator = new BulkDeleteFacultiesCommandValidator();
    }

    [Fact]
    public async Task BulkDeleteFacultiesCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new BulkDeleteFacultiesCommand
        {
            FacultyIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
