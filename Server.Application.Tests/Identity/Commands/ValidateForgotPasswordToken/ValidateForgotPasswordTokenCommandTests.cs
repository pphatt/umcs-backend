using FluentAssertions;

using Server.Application.Features.Identity.Commands.ValidateForgotPasswordToken;
using Server.Contracts.Identity.ValidateForgotPasswordToken;

namespace Server.Application.Tests.Identity.Commands.ValidateForgotPasswordToken;

[Trait("Identity", "Validate Forgot Password Token")]
public class ValidateForgotPasswordTokenCommandTests : BaseTest
{
    [Fact]
    public void ValidateForgotPasswordTokenCommand_ValidateForgotPasswordToken_MapCorrectly()
    {
        // Arrange
        var request = new ValidateForgotPasswordTokenRequest
        {
            Token = ""
        };

        // Act
        var result = _mapper.Map<ValidateForgotPasswordTokenCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(request.Token);
    }
}
