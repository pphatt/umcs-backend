using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Application.Common.Dtos.Identity.Role;
using Server.Application.Common.Extensions;
using Server.Application.Common.Interfaces.Authentication;
using Server.Application.Common.Interfaces.Services;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Entity.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Server.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    IDateTimeProvider _dateTimeProvider;
    UserManager<AppUser> _userManager;
    RoleManager<AppRole> _roleManager;
    JwtSettings _jwtSettings;

    public JwtTokenGenerator(IDateTimeProvider dateTimeProvider, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IOptions<JwtSettings> jwtSettings)
    {
        _dateTimeProvider = dateTimeProvider;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<string> GenerateToken(AppUser user)
    {
        var token = GetEncryptedToken(GetSigningCredentials(), await GetClaims(user));

        return token;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);

        return signingCredentials;
    }

    private async Task<Claim[]> GetClaims(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var permissions = await GetPermissions(roles.ToList());

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(UserClaims.Id, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty),
            new Claim(UserClaims.Roles, string.Join(",", roles)),
            new Claim(UserClaims.Permissions, JsonSerializer.Serialize(permissions)),
            // Jwt ID
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        return claims;
    }

    private async Task<List<string>> GetPermissions(List<string> roles)
    {
        var permissions = new List<string>();

        if (roles.Contains(Roles.Admin))
        {
            var rolePermissions = new List<RoleClaimsDto>();

            var types = typeof(Permissions).GetNestedTypes().ToList();

            types.ForEach(rolePermissions.GetPermissionByType);

            if (rolePermissions.Any())
            {
                permissions = rolePermissions.ConvertAll(permission => permission.Value!);
            }
        }
        else
        {
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role is null)
                {
                    continue;
                }

                var rolePermissions = await _roleManager.GetClaimsAsync(role);

                permissions.AddRange(rolePermissions.Select(p => p.Value!));
            }
        }

        return permissions.Distinct().ToList();
    }

    private string GetEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: _dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            claims: claims,
            signingCredentials: signingCredentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);

        return encryptedToken;
    }
}
