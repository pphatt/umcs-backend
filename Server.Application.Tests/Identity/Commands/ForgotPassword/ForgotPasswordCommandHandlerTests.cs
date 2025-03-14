using FluentAssertions;

using Moq;

using Server.Application.Features.Identity.Commands.ForgotPassword;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Identity.Commands.ForgotPassword;

[Trait("Identity", "Forgot Password")]
public class ForgotPasswordCommandHandlerTests : BaseTest
{
    private readonly ForgotPasswordCommandHandler _commandHandler;

    public ForgotPasswordCommandHandlerTests()
    {
        _commandHandler = new ForgotPasswordCommandHandler(
            _mockUserManager.Object,
            _mockEmailService.Object,
            _mockConfiguration.Object
        );
    }

    [Fact]
    public async Task ForgotPasswordCommandHandler_ForgotPassword_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        var command = new ForgotPasswordCommand
        {
            Email = "nonexistent@example.com"
        };

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
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
    public async Task ForgotPasswordCommandHandler_ForgotPassword_Should_SendEmailSuccessfully_WhenUserExists()
    {
        // Arrange
        var command = new ForgotPasswordCommand
        {
            Email = "user@example.com"
        };

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            UserName = "test-user"
        };

        var resetToken = "reset-token-abc-123";

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(m => m.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync(resetToken);

        _mockEmailService
            .Setup(s => s.SendEmailAsync(It.IsAny<MailRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be($"Sent email to user '{user.UserName}' successfully. Please check email");
    }
}
