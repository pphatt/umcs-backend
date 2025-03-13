using FluentAssertions;

using Moq;

using Server.Application.Features.AcademicYearsApp.Commands.UpdateAcademicYear;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Tests.AcademicYears.UpdateAcademicYear;

using AcademicYear = Server.Domain.Entity.Content.AcademicYear;

[Trait("Academic Year", "Update")]
public class UpdateAcademicYearCommandHandlerTests : BaseTest
{
    private readonly Guid _academicYearId = Guid.NewGuid();
    private readonly string _academicYearName = "2025-2026";
    private readonly UpdateAcademicYearCommandHandler _commandHandler;

    public UpdateAcademicYearCommandHandlerTests()
    {
        var academicYear = new AcademicYear
        {
            Id = _academicYearId,
            Name = _academicYearName,
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
            .Setup(repo => repo.GetAcademicYearByNameAsync(_academicYearName))
            .ReturnsAsync(academicYear);

        _commandHandler = new UpdateAcademicYearCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task UpdateAcademicYearCommandHandler_UpdateAcademicYear_Should_ReturnError_WhenSearchByAcademicYearIdCannotFound()
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            Id = Guid.NewGuid(),
            AcademicYearName = _academicYearName,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(Errors.AcademicYears.CannotFound.Code);
        result.FirstError.Description.Should().Be(Errors.AcademicYears.CannotFound.Description);
    }

    [Fact]
    public async Task UpdateAcademicYearCommandHandler_UpdateAcademicYear_Should_ReturnError_WhenDuplicateAcademicYearFound()
    {
        // Arrange
        var commandId = Guid.NewGuid();

        // we have to have this because:
        // - GetByIdAsync just return "new AcademicYear".
        // - need it to pass the first case.
        var academicYearToUpdate = new AcademicYear
        {
            Id = commandId,
            Name = _academicYearName,
            IsActive = true,
            UserIdCreated = Guid.NewGuid(),
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
            DateCreated = DateTime.UtcNow
        };

        var command = new UpdateAcademicYearCommand
        {
            Id = commandId,
            AcademicYearName = _academicYearName,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.GetByIdAsync(commandId))
            .ReturnsAsync(academicYearToUpdate);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be(Errors.AcademicYears.DuplicateName.Code);
        result.FirstError.Description.Should().Be(Errors.AcademicYears.DuplicateName.Description);
    }

    [Fact]
    public async Task UpdateAcademicYearCommandHandler_UpdateAcademicYear_ShouldUpdateSuccessfully()
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            Id = _academicYearId,
            AcademicYearName = _academicYearName,
            StartClosureDate = _dateTimeProvider.UtcNow.AddDays(1),
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Update academic year successfully.");

        _mockAcademicYearRepository.Verify(
             repo => repo.Update(It.Is<AcademicYear>(ay =>
                 ay.Id == _academicYearId &&
                 ay.Name == _academicYearName &&
                 ay.StartClosureDate == command.StartClosureDate &&
                 ay.EndClosureDate == command.EndClosureDate &&
                 ay.FinalClosureDate == command.FinalClosureDate &&
                 ay.DateUpdated != null)),
             Times.Once);

        _mockUnitOfWork.Verify(
            uow => uow.CompleteAsync(),
            Times.Once);
    }
}
