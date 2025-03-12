using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using Server.Application.Common.Interfaces.Services;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Infrastructure.Tests.UserService;

using UserService = Services.UserService;

public class GetUserIdTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly IUserService _userService;

    public GetUserIdTests()
    {
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _userService = new UserService(_httpContextAccessor.Object);
    }

    [Fact]
    public void GetUserId_Authenticated_ShouldReturnUserId()
    {
        // arrange
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "613DA9F6-FC5A-4E7F-AB2E-7FC89258A596"),
            new Claim(UserClaims.Id, "613DA9F6-FC5A-4E7F-AB2E-7FC89258A596"),
            new Claim(ClaimTypes.NameIdentifier, "test"),
            new Claim(JwtRegisteredClaimNames.Email, "test@gmail.com"),
            new Claim(ClaimTypes.Name, "test-user"),
            new Claim(UserClaims.Roles, string.Join(",", [Roles.Admin, Roles.Student])),
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
        {
            User = user
        });

        // act
        var userId = _userService.GetUserId();

        // assert
        userId.Should().NotBeEmpty();
        userId.Should().Be("613DA9F6-FC5A-4E7F-AB2E-7FC89258A596");
    }

    [Fact]
    public void GetUserId_NotAuthenticated_ShouldReturnNull()
    {
        // cannot pass this case because GetUserId is just only use when authenticated,
        // and the authentication can only be validate in the Authenticate Pipeline by ASP.NET Core and here cannot be replicated that.
        // arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
        {
            User = user
        });

        // act
        var userId = _userService.GetUserId();

        // assert
        userId.Should().BeEmpty();
    }
}
