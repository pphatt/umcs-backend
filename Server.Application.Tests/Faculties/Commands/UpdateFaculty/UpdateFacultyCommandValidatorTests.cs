using FluentValidation.TestHelper;

using Server.Application.Features.FacultyApp.Commands.UpdateFaculty;

namespace Server.Application.Tests.Faculties.Commands.UpdateFaculty;

[Trait("Faculty", "Update")]
public class UpdateFacultyCommandValidatorTests : BaseTest
{
    private readonly UpdateFacultyCommandValidator _validator;

    public UpdateFacultyCommandValidatorTests()
    {
        _validator = new UpdateFacultyCommandValidator();
    }

    [Theory]
    [InlineData("IT")]
    [InlineData("Business")]
    public async Task UpdateFacultyCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid(string name)
    {
        // Arrange
        var command = new UpdateFacultyCommand
        {
            Name = name,
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null, "Name must not be null")]
    [InlineData("", "Name must not be empty")]
    [InlineData("a very long name exceeding 256 characters " +
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz" +
                "abcdefghijklmnopqrstuvwxyz", "Name must not exceed 256 characters")]
    public async Task UpdateFacultyCommandValidator_Should_ReturnError_WhenInvalidName(string? name, string expectedErrorMessage)
    {
        // Arrange
        var command = new UpdateFacultyCommand { Name = name };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
