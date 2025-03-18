using FluentAssertions;

using Server.Application.Features.Identity.Commands.DeleteUser;
using Server.Contracts.Identity.DeleteUser;

namespace Server.Application.Tests.Identity.Commands.DeleteUser;

[Trait("Identity", "Delete")]
public class DeleteUserCommandTests : BaseTest
{
    [Fact]
    public void DeleteUserCommand_DeleteUser_MapCorrectly()
    {
        // Arrange
        var request = new DeleteUserRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        var result = _mapper.Map<DeleteUserCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(request.Id);
    }
}
