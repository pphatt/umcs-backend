using FluentAssertions;

using Server.Application.Features.Users.Commands.CreateUser;
using Server.Contracts.Identity.CreateUser;

namespace Server.Application.Tests.Identity.Commands.CreateUser;

[Trait("Identity", "Create")]
public class CreateUserCommandTests : BaseTest
{
    [Fact]
    public void CreateUserCommand_CreateUser_MapCorrectly()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "test@example.com",
            UserName = "testuser",
            FirstName = "John",
            LastName = "Doe",
            FacultyId = Guid.NewGuid(),
            RoleId = Guid.NewGuid(),
            IsActive = true,
            Avatar = null
        };

        // Act
        var result = _mapper.Map<CreateUserCommand>(request);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
        result.Username.Should().Be(request.UserName);
        result.FirstName.Should().Be(request.FirstName);
        result.LastName.Should().Be(request.LastName);
        result.FacultyId.Should().Be(request.FacultyId);
        result.RoleId.Should().Be(request.RoleId);
        result.IsActive.Should().Be(request.IsActive);
        result.Avatar.Should().Be(request.Avatar);
    }
}
