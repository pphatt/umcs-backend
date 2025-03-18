using FluentAssertions;

using Moq;

using Server.Application.Features.Identity.Commands.ValidateForgotPasswordToken;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Identity.Commands.ValidateForgotPasswordToken;

[Trait("Identity", "Validate Forgot Password Token")]
public class ValidateForgotPasswordTokenCommandHandlerTests : BaseTest
{
    private readonly ValidateForgotPasswordTokenCommandHandler _commandHandler;

    public ValidateForgotPasswordTokenCommandHandlerTests()
    {
        _commandHandler = new ValidateForgotPasswordTokenCommandHandler(
            _mockUserManager.Object,
            _mockDataProtectionProvider.Object
        );
    }

    [Fact]
    public async Task ValidateForgotPasswordTokenCommandHandler_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var token = CreateToken(userId);
        var command = new ValidateForgotPasswordTokenCommand
        {
            Token = Uri.EscapeDataString(token)
        };

        _mockDataProtector
            .Setup(p => p.Unprotect(It.IsAny<byte[]>()))
            .Returns(CreateTokenBytes(userId));

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId))
            .ReturnsAsync((AppUser)null);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.User.CannotFound);
        result.FirstError.Code.Should().Be(Errors.User.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.User.CannotFound.Description);
    }

    [Fact]
    public async Task ValidateForgotPasswordTokenCommandHandler_Should_ReturnTrue_WhenTokenIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = CreateToken(userId.ToString());
        var command = new ValidateForgotPasswordTokenCommand
        {
            Token = Uri.EscapeDataString(token)
        };

        var user = new AppUser { Id = userId };

        _mockDataProtector
            .Setup(p => p.Unprotect(It.IsAny<byte[]>()))
            .Returns(CreateTokenBytes(userId.ToString()));

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(m => m.VerifyUserTokenAsync(user, "DataProtectorTokenProvider", "ResetPassword", token))
            .ReturnsAsync(true);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper<bool>>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Validate forgot password token successfully.");
    }

    [Fact]
    public async Task ValidateForgotPasswordTokenCommandHandler_Should_ReturnFalse_WhenTokenIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = CreateToken(userId.ToString());
        var command = new ValidateForgotPasswordTokenCommand
        {
            Token = Uri.EscapeDataString(token)
        };

        var user = new AppUser { Id = userId };

        _mockDataProtector
            .Setup(p => p.Unprotect(It.IsAny<byte[]>()))
            .Returns(CreateTokenBytes(userId.ToString()));

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(m => m.VerifyUserTokenAsync(user, "DataProtectorTokenProvider", "ResetPassword", token))
            .ReturnsAsync(false);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.IsSuccessful.Should().BeTrue();
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

    private byte[] CreateTokenBytes(string userId)
    {
        using (var ms = new MemoryStream())
        {
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(DateTimeOffset.UtcNow.Ticks);
                writer.Write(userId);
            }
            return ms.ToArray();
        }
    }
}
