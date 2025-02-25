using Microsoft.AspNetCore.Identity;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Entity.Identity;
using System.Security.Claims;

namespace Server.Application.Common.Extensions;

public static class IdentityExtension
{
    public static string GetSpecificClaim(this IEnumerable<Claim> claimsIdentity, string claimType)
    {
        var claim = claimsIdentity.FirstOrDefault(x => x.Type == claimType);

        return (claim != null) ? claim.Value : string.Empty;
    }

    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        // can you ClaimsIdentity approach.
        // var userId = ((ClaimsIdentity)claimsPrincipal.Identity!).GetSpecificClaim(UserClaims.Id);
        var userId = claimsPrincipal.Claims.GetSpecificClaim(UserClaims.Id);

        return Guid.Parse(userId);
    }

    public static Guid GetUserFacultyId(this ClaimsPrincipal claimsPrincipal)
    {
        var facultyId = claimsPrincipal.Claims.GetSpecificClaim(UserClaims.FacultyId);

        return Guid.Parse(facultyId);
    }

    public static string GetUserFacultyName(this ClaimsPrincipal claimsPrincipal)
    {
        var facultyName = claimsPrincipal.Claims.GetSpecificClaim(UserClaims.FacultyName);

        return facultyName;
    }

    public static async Task<List<AppUser>> FindUserInRoleByFacultyIdAsync(this UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, string role, Guid facultyId)
    {
        var userInRole = await userManager.GetUsersInRoleAsync(role);

        return userInRole.Where(x => x.FacultyId == facultyId).ToList();
    }
}
