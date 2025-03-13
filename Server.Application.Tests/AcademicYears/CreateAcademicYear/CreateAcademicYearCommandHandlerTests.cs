using FluentAssertions;

using Moq;

using Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;
using Server.Domain.Common.Errors;

namespace Server.Application.Tests.AcademicYears.CreateAcademicYear;

using AcademicYear = Server.Domain.Entity.Content.AcademicYear;

public class CreateAcademicYearCommandHandlerTests : BaseTest
{
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
        var commandHandler = new CreateAcademicYearCommandHandler(_mockUnitOfWork.Object, _userService);

        var result = await commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(Errors.AcademicYears.InvalidName.Code);
        result.FirstError.Description.Should().Be(Errors.AcademicYears.InvalidName.Description);
    }

    [Theory]
    [InlineData("2025-2026")]
    public async Task CreateAcademicYearCommandHandler_CreateAcademicYear_Should_ReturnError_WhenDuplicateAcademicYearName(string name)
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
        var commandHandler = new CreateAcademicYearCommandHandler(_mockUnitOfWork.Object, _userService);

        var firstResultExecution = await commandHandler.Handle(command, CancellationToken.None);

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

        var secondResultExecution = await commandHandler.Handle(command, CancellationToken.None);

        // Assert
        firstResultExecution.IsError.Should().BeFalse();

        secondResultExecution.IsError.Should().BeTrue();
        secondResultExecution.FirstError.Code.Should().Be(Errors.AcademicYears.DuplicateName.Code);
        secondResultExecution.FirstError.Description.Should().Be(Errors.AcademicYears.DuplicateName.Description);
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
        var commandHandler = new CreateAcademicYearCommandHandler(_mockUnitOfWork.Object, _userService);

        var result = await commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();

        _mockAcademicYearRepository.Verify(
            repo => repo.Add(It.IsAny<AcademicYear>()),
            Times.Once);

        _mockUnitOfWork.Verify(
            uow => uow.CompleteAsync(),
            Times.Once);
    }
}
