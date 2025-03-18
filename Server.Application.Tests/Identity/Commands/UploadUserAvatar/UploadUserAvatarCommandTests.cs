using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using Server.Application.Features.Identity.Commands.UploadUserAvatar;
using Server.Contracts.Identity.UploadUserAvatar;

namespace Server.Application.Tests.Identity.Commands.UploadUserAvatar;

[Trait("Identity", "Upload User Avatar")]
public class UploadUserAvatarCommandTests : BaseTest
{
    [Fact]
    public void UploadUserAvatarCommand_UploadUserAvatar_MapCorrectly()
    {
        // Arrange
        var request = new UploadUserAvatarRequest
        {
            Avatar = new Mock<IFormFile>().Object
        };

        // Act
        var result = _mapper.Map<UploadUserAvatarCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Avatar.Should().BeSameAs(request.Avatar);
    }
}
