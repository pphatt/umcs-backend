using FluentValidation.TestHelper;

using Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;
using Server.Application.Features.FacultyApp.Commands.CreateFaculty;

namespace Server.Application.Tests.Faculties.Commands.CreateFaculty;

[Trait("Faculty", "Create")]
public class CreateFacultyCommandValidatorTests : BaseTest
{
    private readonly CreateFacultyCommandValidator _validator;

    public CreateFacultyCommandValidatorTests()
    {
        _validator = new CreateFacultyCommandValidator();
    }

    [Theory]
    [InlineData("IT")]
    [InlineData("Business")]
    public async Task CreateFacultyCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid(string name)
    {
        // Arrange
        var command = new CreateFacultyCommand
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
    public async Task CreateFacultyCommandValidator_Should_ReturnError_WhenInvalidName(string? name, string expectedErrorMessage)
    {
        // Arrange
        var command = new CreateFacultyCommand { Name = name };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
