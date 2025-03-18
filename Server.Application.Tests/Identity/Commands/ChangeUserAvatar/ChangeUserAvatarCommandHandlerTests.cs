using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Moq;

using Server.Application.Common.Dtos.Media;
using Server.Application.Features.Identity.Commands.ChangeUserAvatar;
using Server.Application.Wrapper;
using Server.Contracts.Common.Media;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Identity.Commands.ChangeUserAvatar;

[Trait("Identity", "Change User Avatar")]
public class ChangeUserAvatarCommandHandlerTests : BaseTest
{
    private readonly ChangeUserAvatarCommandHandler _commandHandler;

    public ChangeUserAvatarCommandHandlerTests()
    {
        _commandHandler = new ChangeUserAvatarCommandHandler(
            _mockUserManager.Object,
            _mockMediaService.Object
        );
    }

    [Fact]
    public async Task ChangeUserAvatarCommandHandler_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        var command = new ChangeUserAvatarCommand
        {
            UserId = Guid.NewGuid(),
            Avatar = new Mock<IFormFile>().Object
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.User.CannotFound);
        result.FirstError.Code.Should().Be(Errors.User.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.User.CannotFound.Description);
    }

    [Fact]
    public async Task ChangeUserAvatarCommandHandler_Should_ReturnError_WhenAvatarNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new ChangeUserAvatarCommand
        {
            UserId = userId,
            Avatar = new Mock<IFormFile>().Object
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
        result.FirstError.Code.Should().Be(Errors.User.AvatarNotFound.Code);
        result.FirstError.Description.Should().Be(Errors.User.AvatarNotFound.Description);
    }

    [Fact]
    public async Task ChangeUserAvatarCommandHandler_Should_ReturnError_WhenUpdateFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var oldAvatarPublicId = "old-avatar-id";
        var command = new ChangeUserAvatarCommand
        {
            UserId = userId,
            Avatar = new Mock<IFormFile>().Object
        };

        var user = new AppUser { Id = userId, AvatarPublicId = oldAvatarPublicId };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _mockMediaService
            .Setup(m => m.RemoveFilesFromCloudinary(It.IsAny<List<DeleteFilesRequest>>()))
            .Returns(Task.CompletedTask);

        var uploadResult = new List<FileDto>
        {
            new FileDto { Path = "new-avatar-path", PublicId = "new-avatar-id" }
        };

        _mockMediaService
            .Setup(m => m.UploadFilesToCloudinary(It.IsAny<List<IFormFile>>(), It.IsAny<FileRequiredParamsDto>()))
            .ReturnsAsync(uploadResult);

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
    public async Task ChangeUserAvatarCommandHandler_Should_ChangeAvatarSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var oldAvatarPublicId = "old-avatar-id";
        var newAvatarPath = "new-avatar-path";
        var newAvatarPublicId = "new-avatar-id";
        var avatarFile = new Mock<IFormFile>().Object;
        var command = new ChangeUserAvatarCommand
        {
            UserId = userId,
            Avatar = avatarFile
        };

        var user = new AppUser { Id = userId, AvatarPublicId = oldAvatarPublicId };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _mockMediaService
            .Setup(m => m.RemoveFilesFromCloudinary(It.IsAny<List<DeleteFilesRequest>>()))
            .Returns(Task.CompletedTask);

        var uploadResult = new List<FileDto>
        {
            new FileDto { Path = newAvatarPath, PublicId = newAvatarPublicId }
        };

        _mockMediaService
            .Setup(m => m.UploadFilesToCloudinary(It.IsAny<List<IFormFile>>(), It.IsAny<FileRequiredParamsDto>()))
            .ReturnsAsync(uploadResult);

        _mockUserManager
            .Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Change user's avatar successfully.");

        user.Avatar.Should().Be(newAvatarPath);
        user.AvatarPublicId.Should().Be(newAvatarPublicId);
    }
}
