using FluentAssertions;

using Moq;

using Server.Application.Features.AcademicYearsApp.Commands.BulkDeleteAcademicYears;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Tests.AcademicYears.BulkDeleteAcademicYears;

[Trait("Academic Year", "Bulk Delete")]
public class BulkDeleteAcademicYearsCommandHandlerTests : BaseTest
{
    private readonly BulkDeleteAcademicYearsCommandHandler _commandHandler;
    private readonly List<AcademicYear> _academicYears;

    public BulkDeleteAcademicYearsCommandHandlerTests()
    {
        _academicYears = new List<AcademicYear>
        {
            new AcademicYear
            {
                Id = Guid.NewGuid(),
                Name = "2024-2025",
                UserIdCreated = Guid.NewGuid(),
                IsActive = true,
                StartClosureDate = _dateTimeProvider.UtcNow.AddYears(-1),
                EndClosureDate = _dateTimeProvider.UtcNow.AddYears(-1).AddMonths(1),
                FinalClosureDate = _dateTimeProvider.UtcNow.AddYears(-1).AddMonths(2),
                DateCreated = _dateTimeProvider.UtcNow.AddYears(-1)
            },
            new AcademicYear
            {
                Id = Guid.NewGuid(),
                Name = "2025-2026",
                UserIdCreated = Guid.NewGuid(),
                IsActive = true,
                StartClosureDate = _dateTimeProvider.UtcNow,
                EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
                FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
                DateCreated = _dateTimeProvider.UtcNow
            }
        };

        foreach (var academicYear in _academicYears)
        {
            _mockAcademicYearRepository
                .Setup(repo => repo.GetByIdAsync(academicYear.Id))
                .ReturnsAsync(academicYear);

            _mockAcademicYearRepository
                .Setup(repo => repo.HasContributionsAsync(academicYear.Id))
                .ReturnsAsync(false);
        }

        _commandHandler = new BulkDeleteAcademicYearsCommandHandler(_mockUnitOfWork.Object, _dateTimeProvider);
    }

    [Fact]
    public async Task BulkDeleteAcademicYearsCommandHandler_BulkDeleteAcademicYears_Should_ReturnError_WhenSearchByAcademicYearIdCannotFound()
    {
        // Arrange
        var missingId = Guid.NewGuid();
        var command = new BulkDeleteAcademicYearsCommand
        {
            AcademicIds = new List<Guid> { _academicYears[0].Id, _academicYears[1].Id, missingId }
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(Errors.AcademicYears.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.AcademicYears.CannotFound.Description);
    }

    [Fact]
    public async Task BulkDeleteAcademicYearsCommandHandler_BulkDeleteAcademicYears_Should_ReturnError_WhenHasContributions()
    {
        // Arrange
        var command = new BulkDeleteAcademicYearsCommand
        {
            AcademicIds = new List<Guid> { _academicYears[0].Id, _academicYears[1].Id }
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.HasContributionsAsync(_academicYears[1].Id))
            .ReturnsAsync(true);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(Errors.AcademicYears.HasContributions.Code);
        result.FirstError.Description.Should().Be(Errors.AcademicYears.HasContributions.Description);
    }

    [Fact]
    public async Task BulkDeleteAcademicYearsCommandHandler_BulkDeleteAcademicYears_ShouldUpdateSuccessfully()
    {
        // Arrange
        var command = new BulkDeleteAcademicYearsCommand
        {
            AcademicIds = new List<Guid> { _academicYears[0].Id, _academicYears[1].Id }
        };

        foreach (var academicYear in _academicYears)
        {
            _mockAcademicYearRepository
                .Setup(repo => repo.HasContributionsAsync(academicYear.Id))
                .ReturnsAsync(false);
        }

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Messages.Should().ContainSingle(m => m == $"Successfully deleted {_academicYears.Count} academic years.");
        result.Value.Messages.Should().ContainSingle(m => m == "Each item is available for recovery.");

        foreach (var ay in _academicYears)
        {
            var academicYear = await _mockAcademicYearRepository.Object.GetByIdAsync(ay.Id);
            academicYear.DateDeleted.Should().NotBeNull();
        }

        _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
    }
}
