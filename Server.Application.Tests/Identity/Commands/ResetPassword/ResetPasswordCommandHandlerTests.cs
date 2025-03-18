using FluentAssertions;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

using Moq;

using Server.Application.Features.Identity.Commands.ResetPassword;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Identity.Commands.ResetPassword;

[Trait("Identity", "Reset Password")]
public class ResetPasswordCommandHandlerTests : BaseTest
{
    private readonly ResetPasswordCommandHandler _commandHandler;

    public ResetPasswordCommandHandlerTests()
    {
        _commandHandler = new ResetPasswordCommandHandler(
            _mockUserManager.Object,
            _mockDataProtectionProvider.Object,
            _mockEmailService.Object
        );
    }

    [Fact]
    public async Task ResetPasswordCommandHandler_ResetPassword_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var token = CreateToken(userId);
        var command = new ResetPasswordCommand
        {
            Token = Uri.EscapeDataString(token),
            Password = "NewPassword123!"
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
    public async Task ResetPasswordCommandHandler_ResetPassword_Should_ReturnError_WhenResetPasswordFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = CreateToken(userId.ToString());
        var command = new ResetPasswordCommand
        {
            Token = Uri.EscapeDataString(token),
            Password = "NewPassword123!"
        };

        var user = new AppUser { Id = userId, Email = "user@example.com" };

        _mockDataProtector
            .Setup(p => p.Unprotect(It.IsAny<byte[]>()))
            .Returns(CreateTokenBytes(userId.ToString()));

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(m => m.ResetPasswordAsync(user, token, command.Password))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.User.FailResetPassword);
        result.FirstError.Code.Should().Be(Errors.User.FailResetPassword.Code);
        result.FirstError.Description.Should().Be(Errors.User.FailResetPassword.Description);
    }

    [Fact]
    public async Task ResetPasswordCommandHandler_ResetPassword_Should_ResetPasswordSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = CreateToken(userId.ToString());
        var command = new ResetPasswordCommand
        {
            Token = Uri.EscapeDataString(token),
            Password = "NewPassword123!"
        };

        var user = new AppUser { Id = userId, Email = "user@example.com" };

        _mockDataProtector
            .Setup(p => p.Unprotect(It.IsAny<byte[]>()))
            .Returns(CreateTokenBytes(userId.ToString()));

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(m => m.ResetPasswordAsync(user, token, command.Password))
            .ReturnsAsync(IdentityResult.Success);

        _mockEmailService
            .Setup(s => s.SendEmailAsync(It.IsAny<MailRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Reset password successfully.");
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
