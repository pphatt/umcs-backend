using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using Server.Application.Features.Identity.Commands.EditUserProfile;
using Server.Contracts.Identity.EditUserProfile;

namespace Server.Application.Tests.Identity.Commands.EditUserProfile;

[Trait("Identity", "Edit User Profile")]
public class EditUserProfileCommandTests : BaseTest
{
    [Fact]
    public void EditUserProfileCommand_EditUserProfile_MapCorrectly()
    {
        // Arrange
        var mockAvatar = new Mock<IFormFile>();
        var request = new EditUserProfileRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Dob = new DateTime(1990, 1, 1),
            PhoneNumber = "1234567890",
            Avatar = mockAvatar.Object
        };

        // Act
        var result = _mapper.Map<EditUserProfileCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Dob.Should().Be(new DateTime(1990, 1, 1));
        result.PhoneNumber.Should().Be("1234567890");
        result.Avatar.Should().BeSameAs(request.Avatar);
    }
}
