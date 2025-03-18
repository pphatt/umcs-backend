using FluentAssertions;

using Microsoft.AspNetCore.Identity;

using Moq;

using Server.Application.Features.Identity.Commands.DeleteUser;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;
using Server.Domain.Entity.Token;

namespace Server.Application.Tests.Identity.Commands.DeleteUser;

[Trait("Identity", "Delete")]
public class DeleteUserCommandHandlerTests : BaseTest
{
    private readonly DeleteUserCommandHandler _commandHandler;

    public DeleteUserCommandHandlerTests()
    {
        _commandHandler = new DeleteUserCommandHandler(
            _mockUserManager.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task DeleteUserCommandHandler_Should_ReturnError_WhenUserNotFound()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Id = Guid.NewGuid()
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
    public async Task DeleteUserCommandHandler_Should_ReturnError_WhenUserHasNoRoles()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Id = Guid.NewGuid()
        };

        var user = new AppUser { Id = command.Id };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Roles.CannotFound);
        result.FirstError.Code.Should().Be(Errors.Roles.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.Roles.CannotFound.Description);
    }

    [Fact]
    public async Task DeleteUserCommandHandler_Should_ReturnError_WhenUserIsAdmin()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Id = Guid.NewGuid()
        };

        var user = new AppUser { Id = command.Id };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        var roles = new List<string> { Roles.Admin };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(roles);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.User.CannotDelete);
        result.FirstError.Code.Should().Be(Errors.User.CannotDelete.Code);
        result.FirstError.Description.Should().Be(Errors.User.CannotDelete.Description);
    }

    [Fact]
    public async Task DeleteUserCommandHandler_Should_ReturnError_WhenRoleRemovalFails()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Id = Guid.NewGuid()
        };

        var user = new AppUser { Id = command.Id };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        var roles = new List<string> { Roles.Student };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(roles);

        var identityErrors = new[] { new IdentityError { Code = "RemoveRoleError", Description = "Failed to remove role" } };

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user, roles))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "RemoveRoleError" && e.Description == "Failed to remove role");
    }

    [Fact]
    public async Task DeleteUserCommandHandler_Should_ReturnError_WhenDeleteFails()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Id = Guid.NewGuid()
        };

        var user = new AppUser { Id = command.Id };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        var roles = new List<string> { Roles.Student };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(roles);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user, roles))
            .ReturnsAsync(IdentityResult.Success);

        var identityErrors = new[] { new IdentityError { Code = "DeleteError", Description = "Failed to delete user" } };
        _mockUserManager
            .Setup(m => m.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "DeleteError" && e.Description == "Failed to delete user");
    }

    [Fact]
    public async Task DeleteUserCommandHandler_Should_DeleteSuccessfully_WithNoTokens()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Id = Guid.NewGuid()
        };

        var user = new AppUser { Id = command.Id };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        var roles = new List<string> { Roles.Student };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(roles);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user, roles))
            .ReturnsAsync(IdentityResult.Success);

        _mockUnitOfWork
            .Setup(u => u.TokenRepository.FindByCondition(x => x.UserId == user.Id))
            .Returns((IEnumerable<RefreshToken>)null);

        _mockUserManager
            .Setup(m => m.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Delete user successfully.");

        _mockUserManager.Verify(m => m.RemoveFromRolesAsync(user, roles), Times.Once);
        _mockUserManager.Verify(m => m.DeleteAsync(user), Times.Once);
        _mockUnitOfWork.Verify(u => u.TokenRepository.RemoveRange(null), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteUserCommandHandler_Should_DeleteSuccessfully_WithTokens()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Id = Guid.NewGuid()
        };

        var user = new AppUser { Id = command.Id };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        var roles = new List<string> { Roles.Student };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(roles);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user, roles))
            .ReturnsAsync(IdentityResult.Success);

        var tokens = new List<RefreshToken> { new RefreshToken { UserId = user.Id } };

        _mockUnitOfWork
            .Setup(u => u.TokenRepository.FindByCondition(x => x.UserId == user.Id))
            .Returns(tokens);

        _mockUnitOfWork
            .Setup(u => u.TokenRepository.RemoveRange(tokens))
            .Verifiable();

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        _mockUserManager
            .Setup(m => m.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Delete user successfully.");

        _mockUserManager.Verify(m => m.RemoveFromRolesAsync(user, roles), Times.Once);
        _mockUnitOfWork.Verify(u => u.TokenRepository.RemoveRange(tokens), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        _mockUserManager.Verify(m => m.DeleteAsync(user), Times.Once);
    }
}
