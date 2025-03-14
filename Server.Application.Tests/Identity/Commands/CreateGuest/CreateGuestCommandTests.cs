using FluentAssertions;

using Server.Application.Features.Identity.Commands.CreateGuest;
using Server.Contracts.Identity.CreateGuest;

namespace Server.Application.Tests.Identity.Commands.CreateGuest;

[Trait("Identity", "Create Guest")]
public class CreateGuestCommandTests : BaseTest
{
    [Fact]
    public void CreateGuestCommand_CreateGuest_MapCorrectly()
    {
        // Arrange
        var request = new CreateGuestRequest
        {
            Email = "test@example.com",
            UserName = "testuser",
            FacultyId = Guid.NewGuid(),
        };

        // Act
        var result = _mapper.Map<CreateGuestCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
        result.Username.Should().Be(request.UserName);
        result.FacultyId.Should().Be(request.FacultyId);
    }
}
