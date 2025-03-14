using FluentValidation.TestHelper;

using Server.Application.Features.Identity.Commands.ValidateForgotPasswordToken;

namespace Server.Application.Tests.Identity.Commands.ValidateForgotPasswordToken;

[Trait("Identity", "Validate Forgot Password Token")]
public class ValidateForgotPasswordTokenCommandValidatorTests : BaseTest
{
    private readonly ValidateForgotPasswordTokenCommandValidator _validator;

    public ValidateForgotPasswordTokenCommandValidatorTests()
    {
        _validator = new ValidateForgotPasswordTokenCommandValidator();
    }

    [Fact]
    public async Task ValidateForgotPasswordTokenCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var token = CreateToken(userId);
        var command = new ValidateForgotPasswordTokenCommand
        {
            Token = Uri.EscapeDataString(token),
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private string CreateToken(string userId)
    {
        using (var ms = new MemoryStream())
        {
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(DateTimeOffset.UtcNow.Ticks);
                writer.Write(userId);
            }

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
