using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Moq;

using Server.Application.Common.Dtos.Media;
using Server.Application.Features.Identity.Commands.UploadUserAvatar;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Identity.Commands.UploadUserAvatar;

[Trait("Identity", "Upload User Avatar")]
public class UploadUserAvatarCommandHandlerTests : BaseTest
{
    private readonly UploadUserAvatarCommandHandler _commandHandler;

    public UploadUserAvatarCommandHandlerTests()
    {
        _commandHandler = new UploadUserAvatarCommandHandler(
            _mockUserManager.Object,
            _mockMediaService.Object
        );
    }

    [Fact]
    public async Task UploadUserAvatarCommandHandler_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        var command = new UploadUserAvatarCommand
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
    public async Task UploadUserAvatarCommandHandler_Should_ReturnError_WhenAvatarAlreadyExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UploadUserAvatarCommand
        {
            UserId = userId,
            Avatar = new Mock<IFormFile>().Object
        };

        var user = new AppUser { Id = userId, AvatarPublicId = "existing-avatar-id" };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.User.AvatarAlreadyExist);
        result.FirstError.Code.Should().Be(Errors.User.AvatarAlreadyExist.Code);
        result.FirstError.Description.Should().Be(Errors.User.AvatarAlreadyExist.Description);
    }

    [Fact]
    public async Task UploadUserAvatarCommandHandler_Should_ReturnError_WhenUpdateFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UploadUserAvatarCommand
        {
            UserId = userId,
            Avatar = new Mock<IFormFile>().Object
        };

        var user = new AppUser { Id = userId, AvatarPublicId = null };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

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
    public async Task UploadUserAvatarCommandHandler_Should_UploadAvatarSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newAvatarPath = "new-avatar-path";
        var newAvatarPublicId = "new-avatar-id";
        var avatarFile = new Mock<IFormFile>().Object;
        var command = new UploadUserAvatarCommand
        {
            UserId = userId,
            Avatar = avatarFile
        };

        var user = new AppUser { Id = userId, AvatarPublicId = null };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        var uploadResult = new List<FileDto>
        {
            new FileDto { Path = "new-avatar-path", PublicId = "new-avatar-id" }
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
        result.Value.Message.Should().Be("Upload new user's avatar successfully.");

        user.Avatar.Should().Be(newAvatarPath);
        user.AvatarPublicId.Should().Be(newAvatarPublicId);
    }
}
