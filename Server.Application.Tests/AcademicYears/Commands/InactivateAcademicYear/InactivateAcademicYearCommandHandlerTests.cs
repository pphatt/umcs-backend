using FluentAssertions;

using Moq;

using Server.Application.Features.AcademicYearsApp.Commands.InactivateAcademicYear;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Tests.AcademicYears.Commands.InactivateAcademicYear;

[Trait("Academic Year", "Inactivate")]
public class InactivateAcademicYearCommandHandlerTests : BaseTest
{
    private readonly InactivateAcademicYearCommandHandler _commandHandler;
    private readonly AcademicYear _academicYear;

    public InactivateAcademicYearCommandHandlerTests()
    {
        _academicYear = new AcademicYear
        {
            Id = Guid.NewGuid(),
            Name = "2025-2026",
            UserIdCreated = Guid.NewGuid(),
            IsActive = true,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
        };

        _mockAcademicYearRepository
            .Setup(repo => repo.GetByIdAsync(_academicYear.Id))
            .ReturnsAsync(_academicYear);

        _commandHandler = new InactivateAcademicYearCommandHandler(_mockUnitOfWork.Object, _dateTimeProvider);
    }

    [Fact]
    public async Task InactivateAcademicYearCommandHandler_InactivateAcademicYear_Should_ReturnError_WhenSearchByAcademicYearIdCannotFound()
    {
        // Arrange
        var command = new InactivateAcademicYearCommand
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
    public async Task InactivateAcademicYearCommandHandler_InactivateAcademicYear_ShouldInactivateSuccessfully()
    {
        // Arrange
        var command = new InactivateAcademicYearCommand
        {
            Id = _academicYear.Id
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Inactivate academic year successfully.");
    }
}
