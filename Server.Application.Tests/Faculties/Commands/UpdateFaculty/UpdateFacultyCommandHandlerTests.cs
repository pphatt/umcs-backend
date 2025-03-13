using FluentAssertions;

using Moq;

using Server.Application.Features.FacultyApp.Commands.UpdateFaculty;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Tests.Faculties.Commands.UpdateFaculty;

[Trait("Faculty", "Update")]
public class UpdateFacultyCommandHandlerTests : BaseTest
{
    private readonly UpdateFacultyCommandHandler _commandHandler;
    private readonly Guid _facultyId = Guid.NewGuid();

    public UpdateFacultyCommandHandlerTests()
    {
        var faculty = new Faculty
        {
            Id = _facultyId,
            Name = "IT"
        };

        _mockFacultyRepository
            .Setup(repo => repo.GetByIdAsync(faculty.Id))
            .ReturnsAsync(faculty);

        _mockFacultyRepository
            .Setup(repo => repo.GetFacultyByNameAsync(faculty.Name))
            .ReturnsAsync(faculty);

        _commandHandler = new UpdateFacultyCommandHandler(_mockUnitOfWork.Object, _dateTimeProvider);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateFacultyCommandHandler_UpdateFaculty_Should_ReturnError_WhenFacultyNameIsNullOrEmpty(string? name)
    {
        // Arrange
        var command = new UpdateFacultyCommand
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Faculty.InvalidName);
        result.FirstError.Code.Should().Be(Errors.Faculty.InvalidName.Code);
        result.FirstError.Description.Should().Be(Errors.Faculty.InvalidName.Description);
    }

    [Theory]
    [InlineData("IT")]
    public async Task UpdateFacultyCommandHandler_UpdateFaculty_Should_ReturnError_WhenFacultyNameIsDuplicated(string name)
    {
        // Arrange
        var command = new UpdateFacultyCommand
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Faculty.DuplicateName);
        result.FirstError.Code.Should().Be(Errors.Faculty.DuplicateName.Code);
        result.FirstError.Description.Should().Be(Errors.Faculty.DuplicateName.Description);
    }

    [Fact]
    public async Task UpdateFacultyCommandHandler_UpdateFaculty_Should_ReturnError_WhenSearchByFacultyIdCannotFound()
    {
        // Arrange
        var command = new UpdateFacultyCommand
        {
            Id = Guid.NewGuid(),
            Name = "Business"
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
    public async Task UpdateFacultyCommandHandler_UpdateFaculty_ShouldUpdateSuccessfully()
    {
        // Arrange
        var command = new UpdateFacultyCommand
        {
            Id = _facultyId,
            Name = "Art"
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Update faculty successfully.");

        _mockFacultyRepository.Verify(
            repo => repo.Update(It.IsAny<Faculty>()),
            Times.Once);

        _mockUnitOfWork.Verify(
            uow => uow.CompleteAsync(),
            Times.Once);
    }
}
