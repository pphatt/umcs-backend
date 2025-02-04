using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Application.Common.Interfaces.Authentication;
using Server.Application.Common.Interfaces.Services;
using Server.Domain.Entity.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Infrastructure.Authorization;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    IDateTimeProvider _dateTimeProvider;
    JwtSettings _jwtSettings;

    public JwtTokenGenerator(IDateTimeProvider dateTimeProvider, IOptions<JwtSettings> jwtSettings)
    {
        _dateTimeProvider = dateTimeProvider;
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(AppUser user)
    {
        var token = GetEncryptedToken(GetSigningCredentials(), GetClaims(user));

        return token;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);

        return signingCredentials;
    }

    private Claim[] GetClaims(AppUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };

        return claims;
    }

    private string GetEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: _dateTimeProvider.UtcNow().AddMinutes(_jwtSettings.ExpiryMinutes),
            claims: claims,
            signingCredentials: signingCredentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);

        return encryptedToken;
    }
}
