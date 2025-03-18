using FluentAssertions;

using Microsoft.AspNetCore.Identity;

using Moq;

using Server.Application.Features.Identity.Commands.DeleteUserAvatar;
using Server.Application.Wrapper;
using Server.Contracts.Common.Media;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Identity.Commands.DeleteUserAvatar;

[Trait("Identity", "Delete User Avatar")]
public class DeleteUserAvatarCommandHandlerTests : BaseTest
{
    private readonly DeleteUserAvatarCommandHandler _commandHandler;

    public DeleteUserAvatarCommandHandlerTests()
    {
        _commandHandler = new DeleteUserAvatarCommandHandler(
            _mockUserManager.Object,
            _mockMediaService.Object
        );
    }

    [Fact]
    public async Task DeleteUserAvatarCommandHandler_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        var command = new DeleteUserAvatarCommand
        {
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.User.CannotFound);
    }

    [Fact]
    public async Task DeleteUserAvatarCommandHandler_Should_ReturnError_WhenAvatarNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserAvatarCommand
        {
            UserId = userId
        };

        var user = new AppUser { Id = userId, AvatarPublicId = null };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.User.AvatarNotFound);
    }

    [Fact]
    public async Task DeleteUserAvatarCommandHandler_Should_ReturnError_WhenUpdateFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var avatarPublicId = "avatar-id";
        var command = new DeleteUserAvatarCommand
        {
            UserId = userId
        };

        var user = new AppUser { Id = userId, AvatarPublicId = "avatar-id" };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _mockMediaService
            .Setup(m => m.RemoveFilesFromCloudinary(It.IsAny<List<DeleteFilesRequest>>()))
            .Returns(Task.CompletedTask);

        var identityErrors = new[] { new IdentityError { Code = "UpdateError", Description = "Failed to update user" } };

        _mockUserManager
            .Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "UpdateError" && e.Description == "Failed to update user");
    }

    [Fact]
    public async Task DeleteUserAvatarCommandHandler_Should_DeleteAvatarSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserAvatarCommand
        {
            UserId = userId
        };

        var user = new AppUser
        {
            Id = userId,
            Avatar = "avatar-path",
            AvatarPublicId = "avatar-id"
        };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _mockMediaService
            .Setup(m => m.RemoveFilesFromCloudinary(It.IsAny<List<DeleteFilesRequest>>()))
            .Returns(Task.CompletedTask);

        _mockUserManager
            .Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Delete avatar successfully.");

        user.Avatar.Should().BeNull();
        user.AvatarPublicId.Should().BeNull();
    }
}
