using FluentValidation.TestHelper;

using Microsoft.AspNetCore.Http;

using Moq;

using Server.Application.Features.Identity.Commands.UploadUserAvatar;

namespace Server.Application.Tests.Identity.Commands.UploadUserAvatar;

[Trait("Identity", "Upload User Avatar")]
public class UploadUserAvatarCommandValidatorTests : BaseTest
{
    private readonly UploadUserAvatarCommandValidator _validator;

    public UploadUserAvatarCommandValidatorTests()
    {
        _validator = new UploadUserAvatarCommandValidator();
    }

    [Fact]
    public async Task UploadUserAvatarCommandValidator_ShouldNot_ReturnError_WhenCommandIsValid()
    {
        // Arrange
        var command = new UploadUserAvatarCommand
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
