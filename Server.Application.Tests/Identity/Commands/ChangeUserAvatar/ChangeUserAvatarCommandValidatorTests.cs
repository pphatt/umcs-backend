using FluentValidation.TestHelper;

using Microsoft.AspNetCore.Http;

using Moq;

using Server.Application.Features.Identity.Commands.ChangeUserAvatar;

namespace Server.Application.Tests.Identity.Commands.ChangeUserAvatar;

[Trait("Identity", "Change User Avatar")]
public class ChangeUserAvatarCommandValidatorTests : BaseTest
{
    private readonly ChangeUserAvatarCommandValidator _validator;

    public ChangeUserAvatarCommandValidatorTests()
    {
        _validator = new ChangeUserAvatarCommandValidator();
    }

    [Fact]
    public async Task ChangeUserAvatarCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new ChangeUserAvatarCommand
        {
            UserId = Guid.NewGuid(),
            Avatar = new Mock<IFormFile>().Object
        };

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
