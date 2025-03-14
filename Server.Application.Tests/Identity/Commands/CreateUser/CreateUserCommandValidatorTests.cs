using FluentValidation.TestHelper;

using Server.Application.Features.Users.Commands.CreateUser;

namespace Server.Application.Tests.Identity.Commands.CreateUser;

[Trait("Identity", "Create")]
public class CreateUserCommandValidatorTests : BaseTest
{
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    [Fact]
    public async Task CreateUserCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Username = "testuser",
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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    public async Task CreateUserCommandValidator_Should_ReturnError_WhenEmailIsInvalid(string? email)
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = email,
            Username = "testuser",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            Dob = _dateTimeProvider.UtcNow.AddYears(-19),
            IsActive = true
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz")] // 260 chars
    public async Task CreateUserCommandValidator_Should_ReturnError_WhenUsernameIsInvalid(string? username)
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Username = username,
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            Dob = _dateTimeProvider.UtcNow.AddYears(-19),
            IsActive = true
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public async Task CreateUserCommandValidator_Should_ReturnError_WhenFirstNameExceedsMaxLength()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Username = "testuser",
            FirstName = new string('A', 257),
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
    public async Task CreateUserCommandValidator_Should_ReturnError_WhenLastNameExceedsMaxLength()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Username = "testuser",
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
    public async Task CreateUserCommandValidator_ShouldNot_ReturnError_WhenDobIsNull()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Username = "testuser",
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
    public async Task CreateUserCommandValidator_Should_ReturnError_WhenDobIsLessThan18Years()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "test@example.com",
            Username = "testuser",
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
