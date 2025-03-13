using FluentAssertions;

using Moq;

using Server.Application.Features.FacultyApp.Commands.CreateFaculty;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Tests.Faculties.Commands.CreateFaculty;

[Trait("Faculty", "Create")]
public class CreateFacultyCommandHandlerTests : BaseTest
{
    private readonly CreateFacultyCommandHandler _commandHandler;

    public CreateFacultyCommandHandlerTests()
    {
        _commandHandler = new CreateFacultyCommandHandler(_mockUnitOfWork.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateFacultyCommandHandler_CreateFaculty_Should_ReturnError_WhenFacultyNameIsNullOrEmpty(string? name)
    {
        // Arrange
        var command = new CreateFacultyCommand
        {
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
    [InlineData("Business")]
    public async Task CreateFacultyCommandHandler_CreateFaculty_Should_ReturnError_WhenFacultyNameIsDuplicated(string name)
    {
        // Arrange
        var command = new CreateFacultyCommand
        {
            Name = name
        };

        var faculty = new Faculty
        {
            Name = name
        };

        _mockFacultyRepository
            .Setup(repo => repo.GetFacultyByNameAsync(faculty.Name))
            .ReturnsAsync(faculty);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Faculty.DuplicateName);
        result.FirstError.Code.Should().Be(Errors.Faculty.DuplicateName.Code);
        result.FirstError.Description.Should().Be(Errors.Faculty.DuplicateName.Description);
    }

    [Theory]
    [InlineData("IT")]
    [InlineData("Business")]
    public async Task CreateFacultyCommandHandler_CreatFaculty_ShouldCreateSuccessfully(string name)
    {
        // Arrange
        var command = new CreateFacultyCommand
        {
            Name = name
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Create new faculty successfully.");

        _mockFacultyRepository.Verify(
            repo => repo.Add(It.IsAny<Faculty>()),
            Times.Once);

        _mockUnitOfWork.Verify(
            uow => uow.CompleteAsync(),
            Times.Once);
    }
}
