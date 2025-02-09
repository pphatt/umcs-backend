using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Interfaces.Services;
using Server.Domain.Entity.Identity;
using System.Security.Claims;

namespace Server.Api.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserService _userService;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public PermissionAuthorizationHandler(IUserService userService, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userService = userService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (_userService.IsAuthenticated() == false)
        {
            context.Fail();
            return;
        }

        var user = await _userManager.FindByNameAsync(context.User.Identity!.Name!);

        if (user is null)
        {
            context.Fail();
            return;
        }

        // get all the user roles.
        var roleNames = await _userManager.GetRolesAsync(user);

        if (!roleNames.Any())
        {
            context.Fail();
            return;
        }

        var permissions = new List<Claim>();

        foreach (var roleName in roleNames)
        {
            // get individual role.
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                context.Fail();
                return;
            }

            // get all role claims on the RoleClaims table.
            var roleClaims = await _roleManager.GetClaimsAsync(role);

            permissions.AddRange(roleClaims);
        }

        // filter it down by:
        // - is the role claim type "permissions" (this is because we can store many other information in this table so make sure we only take permissions.)
        // - check is the require permission for the route is contained in the permissions list.
        var result =
            permissions.Where(
                x => x.Type == "permissions" && 
                x.Value == requirement.Permission && 
                x.Issuer == "LOCAL AUTHORITY"
            );

        if (!result.Any())
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
