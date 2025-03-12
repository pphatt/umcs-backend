using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using Server.Application.Common.Interfaces.Services;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Infrastructure.Tests.Services.UserService;

using UserService = Server.Infrastructure.Services.UserService;

public class IsAuthenticatedTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly IUserService _userService;

    public IsAuthenticatedTests()
    {
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
        _userService = new UserService(_httpContextAccessor.Object);
    }

    [Fact]
    public void IsAuthenticated_Authenticated_ShouldReturnTrue()
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
        var isAuthenticated = _userService.IsAuthenticated();

        // assert
        isAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void IsAuthenticated_NotAuthenticated_ShouldReturnFalse()
    {
        // arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
        {
            User = user
        });

        // act
        var isAuthenticated = _userService.IsAuthenticated();

        // assert
        isAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void IsAuthenticated_NullHttpContext_ShouldReturnNull()
    {
        // arrange
        _httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // act
        var isAuthenticated = _userService.IsAuthenticated();

        // assert
        isAuthenticated.Should().BeNull();
    }
}
