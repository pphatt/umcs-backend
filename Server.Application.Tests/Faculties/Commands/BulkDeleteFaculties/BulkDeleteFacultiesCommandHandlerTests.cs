using FluentAssertions;

using MockQueryable;

using Moq;

using Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculties;
using Server.Application.Features.FacultyApp.Commands.BulkDeleteFaculty;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Application.Tests.Faculties.Commands.BulkDeleteFaculties;

[Trait("Faculty", "Bulk Delete")]
public class BulkDeleteFacultiesCommandHandlerTests : BaseTest
{
    private readonly BulkDeleteFacultiesCommandHandler _commandHandler;
    private readonly List<Faculty> _faculties;

    public BulkDeleteFacultiesCommandHandlerTests()
    {
        _faculties = new List<Faculty>
        {
            new Faculty
            {
                Id = Guid.NewGuid(),
                Name = "IT"
            },
            new Faculty
            {
                Id = Guid.NewGuid(),
                Name = "Business"
            },
            new Faculty
            {
                Id = Guid.NewGuid(),
                Name = "Art"
            },
        };

        foreach (var faculty in _faculties)
        {
            _mockFacultyRepository
                .Setup(repo => repo.GetByIdAsync(faculty.Id))
                .ReturnsAsync(faculty);
        }

        _commandHandler = new BulkDeleteFacultiesCommandHandler(_mockUnitOfWork.Object, _mockUserManager.Object, _dateTimeProvider);
    }

    [Fact]
    public async Task BulkDeleteFacultiesCommandHandler_BulkDeleteFaculties_Should_ReturnError_WhenSearchByFacultyIdCannotFound()
    {
        // Arrange
        var command = new BulkDeleteFacultiesCommand
        {
            FacultyIds = new List<Guid> { Guid.NewGuid() }
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Faculty.CannotFound);
        result.FirstError.Code.Should().Be(Errors.Faculty.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.Faculty.CannotFound.Description);
    }

    [Fact]
    public async Task BulkDeleteFacultiesCommandHandler_BulkDeleteFaculties_Should_ReturnError_WhenHasUsersInFaculty()
    {
        // Arrange
        var command = new BulkDeleteFacultiesCommand
        {
            FacultyIds = new List<Guid> { _faculties[0].Id, _faculties[1].Id }
        };

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            FacultyId = _faculties[0].Id,
            UserName = "testuser",
            Email = "test@example.com",
            IsActive = true,
            DateCreated = DateTime.UtcNow
        };

        var usersList = new List<AppUser> { user };
        var mockUsersQueryable = usersList.AsQueryable().BuildMock();
        _mockUserManager
            .Setup(m => m.Users)
            .Returns(mockUsersQueryable);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Faculty.HasUserIn);
        result.FirstError.Code.Should().Be(Errors.Faculty.HasUserIn.Code);
        result.FirstError.Description.Should().Be(Errors.Faculty.HasUserIn.Description);
    }

    [Fact]
    public async Task BulkDeleteFacultiesCommandHandler_BulkDeleteFaculties_ShouldDeleteSuccessfully()
    {
        // Arrange
        var command = new BulkDeleteFacultiesCommand
        {
            FacultyIds = new List<Guid> { _faculties[0].Id, _faculties[1].Id }
        };

        var userList = new List<AppUser> { };
        var mockUserQueryable = userList.AsQueryable().BuildMock();
        _mockUserManager
            .Setup(m => m.Users)
            .Returns(mockUserQueryable);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Messages.Should().ContainSingle(m => m == $"Successfully deleted {command.FacultyIds.Count} faculties.");
        result.Value.Messages.Should().ContainSingle(m => m == "Each item is available for recovery.");

        _mockUnitOfWork.Verify(
            uow => uow.CompleteAsync(),
            Times.Once);
    }
}
