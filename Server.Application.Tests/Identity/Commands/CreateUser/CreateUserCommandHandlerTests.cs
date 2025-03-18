using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Moq;

using Server.Application.Common.Dtos.Media;
using Server.Application.Features.Users.Commands.CreateUser;
using Server.Application.Wrapper;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Identity.Commands.CreateUser;

[Trait("Identity", "Create")]
public class CreateUserCommandHandlerTests : BaseTest
{
    private readonly CreateUserCommandHandler _commandHandler;

    public CreateUserCommandHandlerTests()
    {
        _commandHandler = new CreateUserCommandHandler(
            _mockUserManager.Object,
            _mockRoleManager.Object,
            _mockEmailService.Object,
            _mockUnitOfWork.Object,
            _mapper,
            _mockMediaService.Object,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task CreateUserCommandHandler_CreateUser_Should_ReturnError_WhenEmailIsDuplicated()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Username = "testuser",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true
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
    public async Task CreateUserCommandHandler_CreateUser_Should_ReturnError_WhenRoleDoesNotExist()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Username = "testuser",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true
        };

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser)null);

        _mockRoleManager
            .Setup(m => m.FindByIdAsync(command.RoleId.ToString()))
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
    public async Task CreateUserCommandHandler_CreateUser_Should_ReturnError_WhenFacultyDoesNotExistForNonAdminOrManager()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Username = "testuser",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true
        };

        var role = new AppRole { Id = command.RoleId, Name = Roles.Student }; // Non-admin/manager role

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser)null);

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
    public async Task CreateUserCommandHandler_CreateUser_Should_CreateSuccessfully_WhenAdminRoleNoFaculty()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "admin@example.com",
            Username = "adminuser",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true
        };

        var role = new AppRole { Id = command.RoleId, Name = Roles.Admin };

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser)null);

        _mockRoleManager
            .Setup(m => m.FindByIdAsync(command.RoleId.ToString()))
            .ReturnsAsync(role);

        // Faculty not required for Admin, so even if null, it’s fine
        _mockFacultyRepository
            .Setup(repo => repo.GetByIdAsync(command.FacultyId))
            .ReturnsAsync((Faculty)null);

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

        _mockUserManager.Verify(m => m.CreateAsync(It.IsAny<AppUser>()), Times.Once);
        _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<AppUser>(), role.Name), Times.Once);
        _mockEmailService.Verify(s => s.SendEmailAsync(It.IsAny<MailRequest>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserCommandHandler_CreateUser_Should_CreateSuccessfully_WithAvatarAndFaculty()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "student@example.com",
            Username = "studentuser",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true,
            Avatar = new Mock<IFormFile>().Object
        };

        var role = new AppRole { Id = command.RoleId, Name = Roles.Student };
        var faculty = new Faculty { Id = command.FacultyId, Name = "Science" };
        var uploadResult = new List<FileDto>
        {
            new FileDto { Path = "avatar_url", PublicId = "avatar_public_id" }
        };

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(command.Email))
            .ReturnsAsync((AppUser)null);

        _mockRoleManager
            .Setup(m => m.FindByIdAsync(command.RoleId.ToString()))
            .ReturnsAsync(role);

        _mockFacultyRepository
            .Setup(repo => repo.GetByIdAsync(command.FacultyId))
            .ReturnsAsync(faculty);

        _mockMediaService
            .Setup(s => s.UploadFilesToCloudinary(It.IsAny<List<IFormFile>>(), It.IsAny<FileRequiredParamsDto>()))
            .ReturnsAsync(uploadResult);

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
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Create new user successfully.");

        _mockMediaService.Verify(s => s.UploadFilesToCloudinary(It.IsAny<List<IFormFile>>(), It.IsAny<FileRequiredParamsDto>()), Times.Once);
        _mockUserManager.Verify(m => m.CreateAsync(It.Is<AppUser>(u => u.Avatar == "avatar_url" && u.AvatarPublicId == "avatar_public_id")), Times.Once);
        _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<AppUser>(), role.Name), Times.Once);
        _mockEmailService.Verify(s => s.SendEmailAsync(It.IsAny<MailRequest>()), Times.Once);
    }
}
