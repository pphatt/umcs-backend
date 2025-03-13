using FluentAssertions;

using Server.Application.Features;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Application.Tests.Identity;

public class CurrentUserTests
{
    // TestMethod_Scenario_ExpectedResult
    [Theory]
    [InlineData(Roles.Admin)]
    [InlineData(Roles.Student)]
    public void IsInRole_WithMatchingRole_ShouldReturnTrue(string role)
    {
        // arrange
        var currentUser = new CurrentUser("1", "test@gmail.com", [Roles.Admin, Roles.Student], null, null);

        // act
        var isInRole = currentUser.IsInRole(role);

        // assert
        isInRole.Should().BeTrue();
    }

    [Fact]
    public void IsInRole_WithNoMatchingRole_ShouldReturnFalse()
    {
        // arrange
        var currentUser = new CurrentUser("1", "test@gmail.com", [Roles.Student], null, null);

        // act
        var isInRole = currentUser.IsInRole(Roles.Admin);

        // assert
        isInRole.Should().BeFalse();
    }

    [Fact]
    public void IsInRole_WithNoMatchingRoleCase_ShouldReturnFalse()
    {
        // arrange
        var currentUser = new CurrentUser("1", "test@gmail.com", [Roles.Student], null, null);

        // act
        var isInRole = currentUser.IsInRole(Roles.Student.ToLower());

        // assert
        isInRole.Should().BeFalse();
    }
}
