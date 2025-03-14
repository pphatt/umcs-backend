using FluentAssertions;

using Server.Application.Features.Identity.Commands.UpdateUser;
using Server.Contracts.Identity.UpdateUser;

namespace Server.Application.Tests.Identity.Commands.UpdateUser;

[Trait("Identity", "Update")]
public class UpdateUserCommandTests : BaseTest
{
    [Fact]
    public void UpdateUserCommand_UpdateUser_MapCorrectly()
    {
        // Arrange
        var request = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true,
        };

        // Act
        var result = _mapper.Map<UpdateUserCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(request.Id);
        result.FirstName.Should().Be(request.FirstName);
        result.LastName.Should().Be(request.LastName);
        result.FacultyId.Should().Be(request.FacultyId);
        result.RoleId.Should().Be(request.RoleId);
        result.IsActive.Should().Be(request.IsActive);
    }
}
