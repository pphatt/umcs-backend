using FluentValidation.TestHelper;

using Server.Application.Features.Identity.Commands.ForgotPassword;

namespace Server.Application.Tests.Identity.Commands.ForgotPassword;

[Trait("Identity", "Forgot Password")]
public class ForgotPasswordCommandValidatorTests : BaseTest
{
    private readonly ForgotPasswordCommandValidator _validator;

    public ForgotPasswordCommandValidatorTests()
    {
        _validator = new ForgotPasswordCommandValidator();
    }

    [Fact]
    public async Task ForgotPasswordCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new ForgotPasswordCommand
        {
            Email = "user@example.com"
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
    public async Task ForgotPasswordCommandValidator_Should_ReturnError_WhenEmailIsInvalid(string? email)
    {
        // Arrange
        var command = new ForgotPasswordCommand
        {
            Email = email
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
}
