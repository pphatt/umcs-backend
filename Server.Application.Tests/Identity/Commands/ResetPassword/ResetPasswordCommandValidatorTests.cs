using FluentValidation.TestHelper;

using Server.Application.Features.Identity.Commands.ResetPassword;

namespace Server.Application.Tests.Identity.Commands.ResetPassword;

[Trait("Identity", "Reset Password")]
public class ResetPasswordCommandValidatorTests : BaseTest
{
    private readonly ResetPasswordCommandValidator _validator;

    public ResetPasswordCommandValidatorTests()
    {
        _validator = new ResetPasswordCommandValidator();
    }

    [Fact]
    public async Task ResetPasswordCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var token = CreateToken(userId);
        var command = new ResetPasswordCommand
        {
            Token = Uri.EscapeDataString(token),
            Password = "NewPassword123!"
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
