using FluentAssertions;

using Moq;

using Server.Application.Features.AcademicYearsApp.Commands.ActivateAcademicYear;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;

namespace Server.Application.Tests.AcademicYears.Commands.ActivateAcademicYear;

[Trait("Academic Year", "Activate")]
public class ActivateAcademicYearCommandHandlerTests : BaseTest
{
    private readonly ActivateAcademicYearCommandHandler _commandHandler;
    private readonly AcademicYear _academicYear;

    public ActivateAcademicYearCommandHandlerTests()
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

        _commandHandler = new ActivateAcademicYearCommandHandler(_mockUnitOfWork.Object, _dateTimeProvider);
    }

    [Fact]
    public async Task ActivateAcademicYearCommandHandler_ActivateAcademicYear_Should_ReturnError_WhenSearchByAcademicYearIdCannotFound()
    {
        // Arrange
        var command = new ActivateAcademicYearCommand
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
    public async Task ActivateAcademicYearCommandHandler_ActivateAcademicYear_ShouldActivateSuccessfully()
    {
        // Arrange
        var command = new ActivateAcademicYearCommand
        {
            Id = _academicYear.Id
        };

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType<ResponseWrapper>();
        result.Value.IsSuccessful.Should().BeTrue();
        result.Value.Message.Should().Be("Activate academic year successfully.");
    }
}
