using FluentAssertions;

using Moq;

using Server.Application.Features.AcademicYearsApp.Commands.DeleteAcademicYear;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Tests.AcademicYears.Commands.DeleteAcademicYear;


[Trait("Academic Year", "Delete")]
public class DeleteAcademicYearCommandHandlerTests : BaseTest
{
    private readonly Guid _academicYearId = Guid.NewGuid();
    private readonly DeleteAcademicYearCommandHandler _commandHandler;

    public DeleteAcademicYearCommandHandlerTests()
    {
        var academicYear = new AcademicYear
        {
            Id = _academicYearId,
            Name = "2025-2026",
            IsActive = true,
            UserIdCreated = Guid.NewGuid(),
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
            DateCreated = DateTime.UtcNow,
            DateUpdated = null
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.GetByIdAsync(_academicYearId))
            .ReturnsAsync(academicYear);

        _mockAcademicYearRepository
            .Setup(repo => repo.HasContributionsAsync(_academicYearId))
            .ReturnsAsync(false);

        _commandHandler = new DeleteAcademicYearCommandHandler(_mockUnitOfWork.Object, _dateTimeProvider);
    }

    [Fact]
    public async Task DeleteAcademicYearCommandHandler_DeleteAcademicYear_Should_ReturnError_WhenSearchByAcademicYearIdCannotFound()
    {
        // Arrange
        var command = new DeleteAcademicYearCommand
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.AcademicYears.CannotFound);
        result.FirstError.Code.Should().Be(Errors.AcademicYears.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.AcademicYears.CannotFound.Description);
    }

    [Fact]
    public async Task DeleteAcademicYearCommandHandler_DeleteAcademicYear_Should_ReturnError_WhenHasContributions()
    {
        // Arrange
        var command = new DeleteAcademicYearCommand
        {
            Id = _academicYearId
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.HasContributionsAsync(command.Id))
            .ReturnsAsync(true);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.AcademicYears.HasContributions);
        result.FirstError.Code.Should().Be(Errors.AcademicYears.HasContributions.Code);
        result.FirstError.Description.Should().Be(Errors.AcademicYears.HasContributions.Description);
    }

    [Fact]
    public async Task DeleteAcademicYearCommandHandler_DeleteAcademicYear_ShouldDeleteSuccessfully()
    {
        // Arrange
        var command = new DeleteAcademicYearCommand
        {
            Id = _academicYearId
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.HasContributionsAsync(command.Id))
            .ReturnsAsync(false);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Delete academic year successfully.");

        // no need to verify update like in the "update-command"
        // because in the "delete-command" just call the "complete-async", while in the "update-command" have the repository update.
        _mockUnitOfWork.Verify(
            uow => uow.CompleteAsync(),
            Times.Once);
    }
}
