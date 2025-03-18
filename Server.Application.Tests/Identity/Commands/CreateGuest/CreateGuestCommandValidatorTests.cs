using FluentValidation.TestHelper;

using Server.Application.Features.Identity.Commands.CreateGuest;

namespace Server.Application.Tests.Identity.Commands.CreateGuest;

[Trait("Identity", "Create Guest")]
public class CreateGuestCommandValidatorTests : BaseTest
{
    private readonly CreateGuestCommandValidator _validator;

    public CreateGuestCommandValidatorTests()
    {
        _validator = new CreateGuestCommandValidator();
    }

    [Fact]
    public async Task CreateGuestCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateGuestCommand
        {
            Email = "guest@example.com",
            Username = "guestuser",
            FacultyId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    public async Task CreateGuestCommandValidator_Should_ReturnError_WhenEmailIsInvalid(string? email)
    {
        // Arrange
        var command = new CreateGuestCommand
        {
            Email = email,
            Username = "guestuser",
            FacultyId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz")] // 260 chars
    public async Task CreateGuestCommandValidator_Should_ReturnError_WhenUsernameIsInvalid(string? username)
    {
        // Arrange
        var command = new CreateGuestCommand
        {
            Email = "guest@example.com",
            Username = username,
            FacultyId = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }
}
