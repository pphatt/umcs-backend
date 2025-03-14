using FluentValidation.TestHelper;

using Server.Application.Features.Identity.Commands.DeleteUser;

namespace Server.Application.Tests.Identity.Commands.DeleteUser;

[Trait("Identity", "Delete")]
public class DeleteUserCommandValidatorTests : BaseTest
{
    private readonly DeleteUserCommandValidator _validator;

    public DeleteUserCommandValidatorTests()
    {
        _validator = new DeleteUserCommandValidator();
    }

    [Fact]
    public async Task DeleteUserCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
