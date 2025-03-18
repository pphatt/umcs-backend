using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using Server.Application.Features.Identity.Commands.ChangeUserAvatar;
using Server.Contracts.Identity.ChangeUserAvatar;

namespace Server.Application.Tests.Identity.Commands.ChangeUserAvatar;

[Trait("Identity", "Change User Avatar")]
public class ChangeUserAvatarCommandTests : BaseTest
{
    [Fact]
    public void ChangeUserAvatarCommand_ChangeUserAvatar_MapCorrectly()
    {
        // Arrange
        var mockAvatar = new Mock<IFormFile>();
        var request = new ChangeUserAvatarRequest
        {
            Avatar = mockAvatar.Object
        };

        // Act
        var result = _mapper.Map<ChangeUserAvatarCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Avatar.Should().BeSameAs(request.Avatar);
    }
}
