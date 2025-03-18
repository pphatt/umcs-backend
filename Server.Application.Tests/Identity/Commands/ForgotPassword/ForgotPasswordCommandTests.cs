using FluentAssertions;

using Server.Application.Features.Identity.Commands.ForgotPassword;
using Server.Contracts.Identity.ForgotPassword;

namespace Server.Application.Tests.Identity.Commands.ForgotPassword;

[Trait("Identity", "Forgot Password")]
public class ForgotPasswordCommandTests : BaseTest
{
    [Fact]
    public void ForgotPasswordCommand_ForgotPassword_MapCorrectly()
    {
        // Arrange
        var request = new ForgotPasswordRequest
        {
            Email = "example@gmail.com"
        };

        // Act
        var result = _mapper.Map<ForgotPasswordCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
    }
}
