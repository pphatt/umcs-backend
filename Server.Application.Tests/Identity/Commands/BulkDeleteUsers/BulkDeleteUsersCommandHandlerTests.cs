using FluentAssertions;

using Microsoft.AspNetCore.Identity;

using Moq;

using Server.Application.Features.Identity.Commands.BulkDeleteUsers;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;
using Server.Domain.Entity.Token;

namespace Server.Application.Tests.Identity.Commands.BulkDeleteUsers;

[Trait("Identity", "Bulk Delete")]
public class BulkDeleteUsersCommandHandlerTests : BaseTest
{
    private readonly BulkDeleteUsersCommandHandler _commandHandler;

    public BulkDeleteUsersCommandHandlerTests()
    {
        _commandHandler = new BulkDeleteUsersCommandHandler(
            _mockUserManager.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task BulkDeleteUsersCommandHandler_BulkDeleteUsers_Should_ReturnError_WhenAnyUserNotFound()
    {
        // Arrange
        var command = new BulkDeleteUsersCommand { UserIds = new List<Guid> { Guid.NewGuid() } };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.User.CannotFound);
        result.FirstError.Code.Should().Be(Errors.User.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.User.CannotFound.Description);
    }

    [Fact]
    public async Task BulkDeleteUsersCommandHandler_BulkDeleteUsers_Should_ReturnError_WhenAnyUserHasNoRoles()
    {
        // Arrange
        var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new BulkDeleteUsersCommand { UserIds = userIds };

        var user1 = new AppUser { Id = userIds[0] };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userIds[0].ToString()))
            .ReturnsAsync(user1);

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user1))
            .ReturnsAsync(new List<string> { });

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Roles.CannotFound);
        result.FirstError.Code.Should().Be(Errors.Roles.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.Roles.CannotFound.Description);
    }

    [Fact]
    public async Task BulkDeleteUsersCommandHandler_BulkDeleteUsers_Should_ReturnError_WhenAnyUserIsAdmin()
    {
        // Arrange
        var users = new List<AppUser>
        {
            new AppUser
            {
                Id = Guid.NewGuid()
            },
            new AppUser
            {
                Id = Guid.NewGuid()
            },
        };

        var command = new BulkDeleteUsersCommand { UserIds = users.Select(x => x.Id).ToList() };

        foreach (var user in users)
        {
            _mockUserManager
                .Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.Admin });
        }

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.User.CannotDelete);
        result.FirstError.Code.Should().Be(Errors.User.CannotDelete.Code);
        result.FirstError.Description.Should().Be(Errors.User.CannotDelete.Description);
    }

    [Fact]
    public async Task BulkDeleteUsersCommandHandler_BulkDeleteUsers_Should_ReturnError_WhenRoleRemovalFails()
    {
        // Arrange
        var users = new List<AppUser>
        {
            new AppUser
            {
                Id = Guid.NewGuid()
            },
            new AppUser
            {
                Id = Guid.NewGuid()
            },
        };
        
        var command = new BulkDeleteUsersCommand { UserIds = users.Select(x => x.Id).ToList() };

        foreach (var user in users)
        {
            _mockUserManager
                .Setup(m => m.FindByIdAsync(user.Id.ToString()))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { Roles.Student });
        }

        var identityErrors = new[] { new IdentityError { Code = "RemoveRoleError", Description = "Failed to remove role" } };

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(users[0], new List<string> { Roles.Student }))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "RemoveRoleError" && e.Description == "Failed to remove role");
    }

    [Fact]
    public async Task BulkDeleteUsersCommandHandler_BulkDeleteUsers_Should_ReturnError_WhenDeleteFails()
    {
        // Arrange
        var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new BulkDeleteUsersCommand { UserIds = userIds };

        var user1 = new AppUser { Id = userIds[0] };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userIds[0].ToString()))
            .ReturnsAsync(user1);

        var user2 = new AppUser { Id = userIds[1] };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userIds[1].ToString()))
            .ReturnsAsync(user2);

        var roles1 = new List<string> { Roles.Student };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user1))
            .ReturnsAsync(roles1);

        var roles2 = new List<string> { Roles.Manager };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user2))
            .ReturnsAsync(roles2);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user1, roles1))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user2, roles2))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.DeleteAsync(user1))
            .ReturnsAsync(IdentityResult.Success);

        var identityErrors = new[] { new IdentityError { Code = "DeleteError", Description = "Failed to delete user" } };

        _mockUserManager
            .Setup(m => m.DeleteAsync(user2))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "DeleteError" && e.Description == "Failed to delete user");
    }

    [Fact]
    public async Task BulkDeleteUsersCommandHandler_BulkDeleteUsers_Should_DeleteSuccessfully_WithNoTokens()
    {
        // Arrange
        var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new BulkDeleteUsersCommand { UserIds = userIds };

        var user1 = new AppUser { Id = userIds[0] };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userIds[0].ToString()))
            .ReturnsAsync(user1);

        var user2 = new AppUser { Id = userIds[1] };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userIds[1].ToString()))
            .ReturnsAsync(user2);

        var roles1 = new List<string> { Roles.Student };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user1))
            .ReturnsAsync(roles1);

        var roles2 = new List<string> { Roles.Manager };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user2))
            .ReturnsAsync(roles2);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user1, roles1))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user2, roles2))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.DeleteAsync(user1))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.DeleteAsync(user2))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be($"Successfully deleted {userIds.Count} users.");

        _mockUserManager.Verify(m => m.RemoveFromRolesAsync(user1, roles1), Times.Once);
        _mockUserManager.Verify(m => m.RemoveFromRolesAsync(user2, roles2), Times.Once);
        _mockUserManager.Verify(m => m.DeleteAsync(user1), Times.Once);
        _mockUserManager.Verify(m => m.DeleteAsync(user2), Times.Once);
        _mockUnitOfWork.Verify(u => u.TokenRepository.RemoveRange(null), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task BulkDeleteUsersCommandHandler_BulkDeleteUsers_Should_DeleteSuccessfully_WithTokens()
    {
        // Arrange
        var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new BulkDeleteUsersCommand { UserIds = userIds };

        var user1 = new AppUser { Id = userIds[0] };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userIds[0].ToString()))
            .ReturnsAsync(user1);

        var user2 = new AppUser { Id = userIds[1] };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(userIds[1].ToString()))
            .ReturnsAsync(user2);

        var roles1 = new List<string> { Roles.Student };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user1))
            .ReturnsAsync(roles1);

        var roles2 = new List<string> { Roles.Manager };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user2))
            .ReturnsAsync(roles2);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user1, roles1))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user2, roles2))
            .ReturnsAsync(IdentityResult.Success);

        var tokens1 = new List<RefreshToken> { new RefreshToken { UserId = user1.Id } };

        _mockUnitOfWork
            .Setup(u => u.TokenRepository.FindByCondition(x => x.UserId == user1.Id))
            .Returns(tokens1);

        _mockUnitOfWork
            .Setup(u => u.TokenRepository.RemoveRange(tokens1))
            .Verifiable();

        var tokens2 = new List<RefreshToken> { new RefreshToken { UserId = user2.Id } };

        _mockUnitOfWork
            .Setup(u => u.TokenRepository.FindByCondition(x => x.UserId == user2.Id))
            .Returns(tokens2);

        _mockUnitOfWork
            .Setup(u => u.TokenRepository.RemoveRange(tokens2))
            .Verifiable();

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        _mockUserManager
            .Setup(m => m.DeleteAsync(user1))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.DeleteAsync(user2))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be($"Successfully deleted {userIds.Count} users.");

        _mockUserManager.Verify(m => m.RemoveFromRolesAsync(user1, roles1), Times.Once);
        _mockUserManager.Verify(m => m.RemoveFromRolesAsync(user2, roles2), Times.Once);
        _mockUnitOfWork.Verify(u => u.TokenRepository.RemoveRange(tokens1), Times.Once);
        _mockUnitOfWork.Verify(u => u.TokenRepository.RemoveRange(tokens2), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Exactly(2));
        _mockUserManager.Verify(m => m.DeleteAsync(user1), Times.Once);
        _mockUserManager.Verify(m => m.DeleteAsync(user2), Times.Once);
    }
}
