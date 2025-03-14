using FluentAssertions;

using Microsoft.AspNetCore.Identity;

using Moq;

using Server.Application.Features.Identity.Commands.CreateGuest;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Identity.Commands.CreateGuest;

[Trait("Identity", "Create Guest")]
public class CreateGuestCommandHandlerTests : BaseTest
{
    private readonly CreateGuestCommandHandler _commandHandler;

    public CreateGuestCommandHandlerTests()
    {
        _commandHandler = new CreateGuestCommandHandler(
            _mockUserManager.Object,
            _mockRoleManager.Object,
            _mockEmailService.Object,
            _mockUnitOfWork.Object,
            _mapper,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task CreateGuestCommandHandler_CreateGuest_Should_ReturnError_WhenEmailIsDuplicated()
    {
        // Arrange
        var command = new CreateGuestCommand
        {
            Email = "guest@example.com",
            Username = "guestuser",
            FacultyId = Guid.NewGuid()
        };

        var existingUser = new AppUser { Email = command.Email };

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.User.DuplicateEmail);
        result.FirstError.Code.Should().Be(Errors.User.DuplicateEmail.Code);
        result.FirstError.Description.Should().Be(Errors.User.DuplicateEmail.Description);
    }

    [Fact]
    public async Task CreateGuestCommandHandler_CreateGuest_Should_ReturnError_WhenGuestRoleDoesNotExist()
    {
        // Arrange
        var command = new CreateGuestCommand
        {
            Email = "guest@example.com",
            Username = "guestuser",
            FacultyId = Guid.NewGuid()
        };

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser)null);

        _mockRoleManager
            .Setup(m => m.FindByNameAsync(Roles.Guest))
            .ReturnsAsync((AppRole)null);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Roles.CannotFound);
        result.FirstError.Code.Should().Be(Errors.Roles.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.Roles.CannotFound.Description);
    }

    [Fact]
    public async Task CreateGuestCommandHandler_CreateGuest_Should_ReturnError_WhenFacultyDoesNotExistForGuest()
    {
        // Arrange
        var command = new CreateGuestCommand
        {
            Email = "guest@example.com",
            Username = "guestuser",
            FacultyId = Guid.NewGuid()
        };

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser)null);

        var role = new AppRole { Name = Roles.Guest };

        _mockRoleManager
            .Setup(m => m.FindByNameAsync(Roles.Guest))
            .ReturnsAsync(role);

        _mockUnitOfWork
            .Setup(u => u.FacultyRepository.GetByIdAsync(command.FacultyId))
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
    public async Task CreateGuestCommandHandler_CreateGuest_Should_ReturnError_WhenUserCreationFails()
    {
        // Arrange
        var command = new CreateGuestCommand
        {
            Email = "guest@example.com",
            Username = "guestuser",
            FacultyId = Guid.NewGuid()
        };

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser)null);

        var role = new AppRole { Name = Roles.Guest };

        _mockRoleManager
            .Setup(m => m.FindByNameAsync(Roles.Guest))
            .ReturnsAsync(role);

        var faculty = new Faculty { Id = command.FacultyId, Name = "IT" };

        _mockUnitOfWork
            .Setup(u => u.FacultyRepository.GetByIdAsync(command.FacultyId))
            .ReturnsAsync(faculty);

        var identityErrors = new[] { new IdentityError { Code = "CreateError", Description = "Failed to create user" } };

        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "CreateError" && e.Description == "Failed to create user");
    }

    [Fact]
    public async Task CreateGuestCommandHandler_CreateGuest_Should_ReturnError_WhenRoleAssignmentFails()
    {
        // Arrange
        var command = new CreateGuestCommand
        {
            Email = "guest@example.com",
            Username = "guestuser",
            FacultyId = Guid.NewGuid()
        };

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser)null);

        var role = new AppRole { Name = Roles.Guest };

        _mockRoleManager
            .Setup(m => m.FindByNameAsync(Roles.Guest))
            .ReturnsAsync(role);

        var faculty = new Faculty { Id = command.FacultyId, Name = "IT" };

        _mockUnitOfWork
            .Setup(u => u.FacultyRepository.GetByIdAsync(command.FacultyId))
            .ReturnsAsync(faculty);

        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var identityErrors = new[] { new IdentityError { Code = "RoleError", Description = "Failed to assign role" } };

        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), role.Name))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Code == "RoleError" && e.Description == "Failed to assign role");
    }

    [Fact]
    public async Task CreateGuestCommandHandler_CreateGuest_Should_CreateSuccessfully()
    {
        // Arrange
        var command = new CreateGuestCommand
        {
            Email = "guest@example.com",
            Username = "guestuser",
            FacultyId = Guid.NewGuid()
        };

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser)null);

        var role = new AppRole { Name = Roles.Guest };

        _mockRoleManager
            .Setup(m => m.FindByNameAsync(Roles.Guest))
            .ReturnsAsync(role);

        var faculty = new Faculty { Id = command.FacultyId, Name = "IT" };

        _mockUnitOfWork
            .Setup(u => u.FacultyRepository.GetByIdAsync(command.FacultyId))
            .ReturnsAsync(faculty);

        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), role.Name))
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
        result.Value.Message.Should().Be("Create new user successfully.");
    }
}
