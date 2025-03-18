using FluentAssertions;

using Moq;

using Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Tests.AcademicYears.Commands.CreateAcademicYear;

using AcademicYear = Domain.Entity.Content.AcademicYear;

[Trait("Academic Year", "Create")]
public class CreateAcademicYearCommandHandlerTests : BaseTest
{
    private readonly CreateAcademicYearCommandHandler _commandHandler;

    public CreateAcademicYearCommandHandlerTests()
    {
        _commandHandler = new CreateAcademicYearCommandHandler(_mockUnitOfWork.Object, _userService);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateAcademicYearCommandHandler_CreateAcademicYear_Should_ReturnError_WhenAcademicYearNameIsNullOrEmpty(string? name)
    {
        // Arrange
        var command = new CreateAcademicYearCommand
        {
            Name = name,
            IsActive = true,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.AcademicYears.InvalidName);
        result.FirstError.Code.Should().Be(Errors.AcademicYears.InvalidName.Code);
        result.FirstError.Description.Should().Be(Errors.AcademicYears.InvalidName.Description);
    }

    [Theory]
    [InlineData("2025-2026")]
    public async Task CreateAcademicYearCommandHandler_CreateAcademicYear_Should_ReturnError_WhenAcademicYearNameIsDuplicated(string name)
    {
        // Arrange
        var command = new CreateAcademicYearCommand
        {
            Name = name,
            IsActive = true,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
        };

        // the reason why need to check this.
        // - https://grok.com/share/bGVnYWN5_2987e7d3-c13e-41d7-a9ea-bef18ea1f35a
        var existingAcademicYear = new AcademicYear
        {
            Name = name,
            IsActive = true,
            UserIdCreated = Guid.NewGuid(),
            StartClosureDate = command.StartClosureDate,
            EndClosureDate = command.EndClosureDate,
            FinalClosureDate = command.FinalClosureDate
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.GetAcademicYearByNameAsync(name))
            .ReturnsAsync(existingAcademicYear);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.AcademicYears.DuplicateName);
        result.FirstError.Code.Should().Be(Errors.AcademicYears.DuplicateName.Code);
        result.FirstError.Description.Should().Be(Errors.AcademicYears.DuplicateName.Description);
    }

    [Fact]
    public async Task CreateAcademicYearCommandHandler_CreateAcademicYear_ShouldCreateSuccessfully()
    {
        // Arrange
        var command = new CreateAcademicYearCommand
        {
            Name = "2025-2026",
            IsActive = true,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Create new academic year successfully.");

        _mockAcademicYearRepository.Verify(
            repo => repo.Add(It.IsAny<AcademicYear>()),
            Times.Once);

        _mockUnitOfWork.Verify(
            uow => uow.CompleteAsync(),
            Times.Once);
    }
}
