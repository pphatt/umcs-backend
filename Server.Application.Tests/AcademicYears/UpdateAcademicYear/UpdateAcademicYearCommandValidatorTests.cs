using FluentValidation.TestHelper;

using Server.Application.Features.AcademicYearsApp.Commands.UpdateAcademicYear;

namespace Server.Application.Tests.AcademicYears.UpdateAcademicYear;

[Trait("Academic Year", "Update")]
public class UpdateAcademicYearCommandValidatorTests : BaseTest
{
    private readonly UpdateAcademicYearCommandValidator _validator;

    public UpdateAcademicYearCommandValidatorTests()
    {
        _validator = new UpdateAcademicYearCommandValidator();
    }

    [Fact]
    public async Task UpdateAcademicYearCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            Id = Guid.NewGuid(),
            AcademicYearName = "2025-2026",
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2),
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task UpdateAcademicYearCommandValidator_UpdateAcademicYear_Should_ReturnError_WhenNameIsEmpty(string? name)
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = name,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.AcademicYearName)
            .WithErrorMessage("Academic year name is required.");
    }

    [Theory]
    [InlineData("2024")]
    [InlineData("2024-")]
    [InlineData("-2025")]
    [InlineData("202-2025")]
    [InlineData("2024-202")]
    [InlineData("ABCD-EFGH")]
    [InlineData("2024-2025-2026")]
    public async Task UpdateAcademicYearCommandValidator_Should_ReturnError_WhenNameFormatIsInvalid(string name)
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = name,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.AcademicYearName)
            .WithErrorMessage("Academic year name must be in the format 'XXXX-YYYY'.");
    }

    [Theory]
    [InlineData("2024-2024")]
    [InlineData("2024-2026")]
    [InlineData("2025-2024")]
    public async Task UpdateAcademicYearCommandValidator_Should_ReturnError_WhenYearsAreNotConsecutive(string name)
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = name,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.AcademicYearName)
            .WithErrorMessage("The academic year must consist of two consecutive years (e.g., 2024-2025).");
    }

    [Theory]
    [InlineData(default)]
    public async Task UpdateAcademicYearCommandValidator_Should_ReturnError_WhenStartClosureDateIsEmpty(DateTime startClosureDate)
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = "2024-2025",
            StartClosureDate = startClosureDate,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.StartClosureDate)
            .WithErrorMessage("StartClosureDate is required.");
    }

    [Theory]
    [InlineData("2025-2026", "2024-12-31")]
    public async Task UpdateAcademicYearCommandValidator_Should_ReturnError_WhenStartClosureDateIsOutsideAcademicYear(string name, string startClosureDate)
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = name,
            StartClosureDate = DateTime.Parse(startClosureDate),
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.StartClosureDate)
            .WithErrorMessage("StartClosureDate must be within the academic year.");
    }

    [Theory]
    [InlineData(default)]
    public async Task UpdateAcademicYearCommandValidator_Should_ReturnError_WhenEndClosureDateIsEmpty(DateTime endClosureDate)
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = "2025-2026",
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = endClosureDate,
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.EndClosureDate)
            .WithErrorMessage("EndClosureDate is required.");
    }

    [Fact]
    public async Task UpdateAcademicYearCommandValidator_Should_ReturnError_WhenEndClosureDateIsBeforeStartClosureDate()
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = "2024-2025",
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddDays(-1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.EndClosureDate)
            .WithErrorMessage("EndClosureDate have to be after StartClosureDate.");
    }

    [Theory]
    [InlineData("2025-2026", "2024-12-31")]
    public async Task UpdateAcademicYearCommandValidator_Should_ReturnError_WhenEndClosureDateIsOutsideAcademicYear(string name, string endClosureDate)
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = name,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = DateTime.Parse(endClosureDate),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(2)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.EndClosureDate)
            .WithErrorMessage("EndClosureDate must be within the academic year or exactly at its end.");
    }

    [Theory]
    [InlineData(default)]
    public async Task UpdateAcademicYearCommandValidator_Should_ReturnError_WhenFinalClosureDateIsEmpty(DateTime finalClosureDate)
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = "2024-2025",
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = finalClosureDate
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.FinalClosureDate)
            .WithErrorMessage("FinalClosureDate is required.");
    }

    [Theory]
    [InlineData("2025-2026", "2024-12-31")]
    public async Task UpdateAcademicYearCommandValidator_Should_ReturnError_WhenFinalClosureDateIsOutsideAcademicYear(string name, string finalClosureDate)
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = name,
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = DateTime.Parse(finalClosureDate)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.FinalClosureDate)
            .WithErrorMessage("FinalClosureDate must be within the academic year or exactly at its end.");
    }

    [Fact]
    public async Task UpdateAcademicYearCommandValidator_Should_ReturnError_WhenFinalClosureDateIsBeforeStartClosureDate()
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = "2025-2026",
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddMonths(-2)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.FinalClosureDate)
            .WithErrorMessage("FinalClosureDate have to be after StartClosureDate.");
    }

    [Fact]
    public async Task UpdateAcademicYearCommandValidator_Should_ReturnError_WhenFinalClosureDateIsBeforeEndClosureDate()
    {
        // Arrange
        var command = new UpdateAcademicYearCommand
        {
            AcademicYearName = "2024-2025",
            StartClosureDate = _dateTimeProvider.UtcNow,
            EndClosureDate = _dateTimeProvider.UtcNow.AddMonths(1),
            FinalClosureDate = _dateTimeProvider.UtcNow.AddDays(1)
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.FinalClosureDate)
            .WithErrorMessage("FinalClosureDate have to be after EndClosureDate.");
    }
}
