using FluentAssertions;

using Server.Application.Features.Identity.Commands.ResetPassword;
using Server.Contracts.Identity.ResetPassword;

namespace Server.Application.Tests.Identity.Commands.ResetPassword;

[Trait("Identity", "Reset Password")]
public class ResetPasswordCommandTests : BaseTest
{
    [Fact]
    public void ResetPasswordCommand_ResetPassword_MapCorrectly()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Token = "",
            Password = ""
        };

        // Act
        var result = _mapper.Map<ResetPasswordCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(request.Token);
        result.Password.Should().Be(request.Password);
    }
}
