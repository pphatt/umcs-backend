using CloudinaryDotNet.Actions;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Moq;

using Server.Application.Common.Dtos.Media;
using Server.Application.Features.Identity.Commands.EditUserProfile;
using Server.Contracts.Common.Media;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Identity.Commands.EditUserProfile;

[Trait("Identity", "Edit User Profile")]
public class EditUserProfileCommandHandlerTests : BaseTest
{
    private readonly EditUserProfileCommandHandler _commandHandler;

    public EditUserProfileCommandHandlerTests()
    {
        _commandHandler = new EditUserProfileCommandHandler(
            _mockUserManager.Object,
            _mockMediaService.Object
        );
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        var command = new EditUserProfileCommand
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
    public async Task Handle_Should_UpdateProfileSuccessfully_WithoutAvatar()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new AppUser
        {
            Id = userId
        };

        var command = new EditUserProfileCommand
        {
            UserId = userId,
            FirstName = "John",
            LastName = "Doe",
            Dob = new DateTime(1990, 1, 1),
            PhoneNumber = "1234567890",
            Avatar = null
        };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Update profile successfully.");
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Dob.Should().Be(new DateTime(1990, 1, 1));
    }

    [Fact]
    public async Task Handle_Should_UpdateProfileSuccessfully_WithAvatar()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new AppUser
        {
            Id = userId,
            AvatarPublicId = "old-avatar-id"
        };

        var mockAvatar = new Mock<IFormFile>();
        var command = new EditUserProfileCommand
        {
            UserId = userId,
            FirstName = "John",
            Avatar = mockAvatar.Object
        };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);

        _mockMediaService
            .Setup(m => m.RemoveFilesFromCloudinary(It.IsAny<List<DeleteFilesRequest>>()))
            .Returns(Task.CompletedTask);

        var uploadResult = new List<FileDto>
        {
            new FileDto { Path = "new-path", PublicId = "new-id" }
        };

        _mockMediaService
            .Setup(m => m.UploadFilesToCloudinary(It.IsAny<List<IFormFile>>(), It.IsAny<FileRequiredParamsDto>()))
            .ReturnsAsync(uploadResult);

        _mockUserManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.IsSuccessful.Should().BeTrue();
        user.FirstName.Should().Be("John");
        user.Avatar.Should().Be("new-path");
        user.AvatarPublicId.Should().Be("new-id");
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenUpdateFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new AppUser
        {
            Id = userId
        };

        var command = new EditUserProfileCommand
        {
            UserId = userId,
            FirstName = "John"
        };

        var errors = new[] { new IdentityError { Code = "UpdateError", Description = "Update failed" } };

        _mockUserManager.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "UpdateError" && e.Description == "Update failed");
    }
}
