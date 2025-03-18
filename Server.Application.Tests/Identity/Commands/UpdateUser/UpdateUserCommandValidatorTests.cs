using FluentValidation.TestHelper;

using Server.Application.Features.Identity.Commands.UpdateUser;

namespace Server.Application.Tests.Identity.Commands.UpdateUser;

[Trait("Identity", "Update")]
public class UpdateUserCommandValidatorTests : BaseTest
{
    private readonly UpdateUserCommandValidator _validator;

    public UpdateUserCommandValidatorTests()
    {
        _validator = new UpdateUserCommandValidator();
    }

    [Fact]
    public async Task UpdateUserCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            Dob = _dateTimeProvider.UtcNow.AddYears(-19),
            IsActive = true
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateUserCommandValidator_Should_ReturnError_WhenFirstNameExceedsMaxLength()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = new string('A', 257),
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            Dob = _dateTimeProvider.UtcNow.AddYears(-19),
            IsActive = true
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async Task UpdateUserCommandValidator_Should_ReturnError_WhenLastNameExceedsMaxLength()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = new string('B', 257),
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            Dob = _dateTimeProvider.UtcNow.AddYears(-19),
            IsActive = true
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public async Task UpdateUserCommandValidator_ShouldNot_ReturnError_WhenDobIsNull()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            Dob = null,
            IsActive = true
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dob);
    }

    [Fact]
    public async Task UpdateUserCommandValidator_Should_ReturnError_WhenDobIsLessThan18Years()
    {
        // Arrange
        var command = new UpdateUserCommand
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            Dob = _dateTimeProvider.UtcNow.AddYears(-17),
            IsActive = true
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dob);
    }
}
