using FluentAssertions;

using Microsoft.AspNetCore.Identity;

using Moq;

using Server.Application.Features.Identity.Commands.UpdateUser;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Identity.Commands.UpdateUser;

[Trait("Identity", "Update")]
public class UpdateUserCommandHandlerTests : BaseTest
{
    private readonly UpdateUserCommandHandler _commandHandler;

    public UpdateUserCommandHandlerTests()
    {
        _commandHandler = new UpdateUserCommandHandler(
            _mockUserManager.Object,
            _mockRoleManager.Object,
            _mockUnitOfWork.Object,
            _mapper
        );
    }

    [Fact]
    public async Task UpdateUserCommandHandler_UpdateUser_Should_ReturnError_WhenSearchByUserIdNotFound()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true,
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
    public async Task UpdateUserCommandHandler_UpdateUser_Should_ReturnError_WhenSearchByRoleIdNotFound()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true,
        };

        var user = new AppUser { Id = command.Id };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Roles.CannotFound);
        result.FirstError.Code.Should().Be(Errors.Roles.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.Roles.CannotFound.Description);
    }

    [Fact]
    public async Task UpdateUserCommandHandler_UpdateUser_Should_ReturnError_WhenSearchByFacultyIdNotFound()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true
        };

        var user = new AppUser { Id = command.Id };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        var role = new AppRole { Id = command.RoleId, Name = Roles.Student };

        _mockRoleManager
            .Setup(m => m.FindByIdAsync(command.RoleId.ToString()))
            .ReturnsAsync(role);

        _mockFacultyRepository
            .Setup(repo => repo.GetByIdAsync(command.FacultyId))
            .ReturnsAsync((Faculty)null);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Faculty.CannotFound);
        result.FirstError.Code.Should().Be(Errors.Faculty.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.Faculty.CannotFound.Description);
    }

    [Fact]
    public async Task UpdateUserCommandHandler_UpdateUser_Should_ReturnError_WhenRoleRemovalFails()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true
        };

        var user = new AppUser { Id = command.Id };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        var role = new AppRole { Id = command.RoleId, Name = Roles.Student };

        _mockRoleManager
            .Setup(m => m.FindByIdAsync(command.RoleId.ToString()))
            .ReturnsAsync(role);

        var faculty = new Faculty { Id = command.FacultyId, Name = "IT" };

        _mockFacultyRepository
            .Setup(repo => repo.GetByIdAsync(command.FacultyId))
            .ReturnsAsync(faculty);

        var currentRoles = new List<string> { Roles.Student };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(currentRoles);

        var identityErrors = new[] { new IdentityError { Code = "RemoveRoleError", Description = "Failed to remove role" } };

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user, currentRoles))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "RemoveRoleError" && e.Description == "Failed to remove role");
    }

    [Fact]
    public async Task UpdateUserCommandHandler_UpdateUser_Should_ReturnError_WhenRoleAdditionFails()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true
        };

        var user = new AppUser { Id = command.Id };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        var role = new AppRole { Id = command.RoleId, Name = Roles.Student };

        _mockRoleManager
            .Setup(m => m.FindByIdAsync(command.RoleId.ToString()))
            .ReturnsAsync(role);

        var faculty = new Faculty { Id = command.FacultyId, Name = "IT" };

        _mockFacultyRepository
            .Setup(repo => repo.GetByIdAsync(command.FacultyId))
            .ReturnsAsync(faculty);

        var currentRoles = new List<string> { Roles.Student };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(currentRoles);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user, currentRoles))
            .ReturnsAsync(IdentityResult.Success);

        var identityErrors = new[] { new IdentityError { Code = "AddRoleError", Description = "Failed to add role" } };

        _mockUserManager
            .Setup(m => m.AddToRoleAsync(user, role.Name))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "AddRoleError" && e.Description == "Failed to add role");
    }

    [Fact]
    public async Task UpdateUserCommandHandler_UpdateUser_Should_ReturnError_WhenUpdateFails()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true
        };

        var user = new AppUser { Id = command.Id };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        var role = new AppRole { Id = command.RoleId, Name = Roles.Student };

        _mockRoleManager
            .Setup(m => m.FindByIdAsync(command.RoleId.ToString()))
            .ReturnsAsync(role);

        var faculty = new Faculty { Id = command.FacultyId, Name = "IT" };

        _mockFacultyRepository
            .Setup(repo => repo.GetByIdAsync(command.FacultyId))
            .ReturnsAsync(faculty);

        var currentRoles = new List<string> { Roles.Student };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(currentRoles);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user, currentRoles))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.AddToRoleAsync(user, role.Name))
            .ReturnsAsync(IdentityResult.Success);

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
    public async Task UpdateUserCommandHandler_UpdateUser_Should_UpdateSuccessfully()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true
        };

        var user = new AppUser
        {
            Id = command.Id,
            FirstName = "Hello",
            LastName = "World",
            FacultyId = Guid.NewGuid()
        };

        _mockUserManager
            .Setup(m => m.FindByIdAsync(command.Id.ToString()))
            .ReturnsAsync(user);

        var role = new AppRole { Id = command.RoleId, Name = Roles.Student };

        _mockRoleManager
            .Setup(m => m.FindByIdAsync(command.RoleId.ToString()))
            .ReturnsAsync(role);

        var faculty = new Faculty { Id = command.FacultyId, Name = "IT" };

        _mockFacultyRepository
            .Setup(repo => repo.GetByIdAsync(command.FacultyId))
            .ReturnsAsync(faculty);

        var currentRoles = new List<string> { Roles.Student };

        _mockUserManager
            .Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(currentRoles);

        _mockUserManager
            .Setup(m => m.RemoveFromRolesAsync(user, currentRoles))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.AddToRoleAsync(user, role.Name))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Update user successfully.");

        _mockUserManager.Verify(m => m.RemoveFromRolesAsync(user, currentRoles), Times.Once);
        _mockUserManager.Verify(m => m.AddToRoleAsync(user, role.Name), Times.Once);
        _mockUserManager.Verify(m => m.UpdateAsync(It.Is<AppUser>(u =>
            u.Id == command.Id &&
            u.FirstName == command.FirstName &&
            u.LastName == command.LastName &&
            u.FacultyId == command.FacultyId &&
            u.IsActive == command.IsActive)), Times.Once);
    }
}
