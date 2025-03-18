using FluentValidation.TestHelper;

using Server.Application.Features.Identity.Commands.DeleteUserAvatar;

namespace Server.Application.Tests.Identity.Commands.DeleteUserAvatar;

[Trait("Identity", "Delete User Avatar")]
public class DeleteUserAvatarCommandValidatorTests : BaseTest
{
    private readonly DeleteUserAvatarCommandValidator _validator;

    public DeleteUserAvatarCommandValidatorTests()
    {
        _validator = new DeleteUserAvatarCommandValidator();
    }

    [Fact]
    public async Task DeleteUserAvatarCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new DeleteUserAvatarCommand
        {
            UserId = Guid.NewGuid(),
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
