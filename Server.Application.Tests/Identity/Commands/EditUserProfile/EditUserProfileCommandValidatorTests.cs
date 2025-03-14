using FluentValidation.TestHelper;

using Server.Application.Features.Identity.Commands.EditUserProfile;

namespace Server.Application.Tests.Identity.Commands.EditUserProfile;

[Trait("Identity", "Edit User Profile")]
public class EditUserProfileCommandValidatorTests : BaseTest
{
    private readonly EditUserProfileCommandValidator _validator;

    public EditUserProfileCommandValidatorTests()
    {
        _validator = new EditUserProfileCommandValidator();
    }

    [Fact]
    public async Task Validator_ShouldNotHaveErrors_WhenFieldsAreValid()
    {
        // Arrange
        var command = new EditUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz")]
    public async Task Validator_ShouldHaveErrors_WhenFirstNameIsInvalid(string? firstName)
    {
        // Arrange
        var command = new EditUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            FirstName = firstName,
            LastName = "Doe"
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz")]
    public async Task Validator_ShouldHaveErrors_WhenLastNameIsInvalid(string? lastName)
    {
        // Arrange
        var command = new EditUserProfileCommand
        {
            UserId = Guid.NewGuid(),
            FirstName = "John",
            LastName = lastName
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}
