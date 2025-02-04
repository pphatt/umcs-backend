using Server.Application.Common.Interfaces.Authentication;
using System.Security.Cryptography;

namespace Server.Infrastructure.Authorization;

public class TokenService : ITokenService
{
    public string GenerateRefreshToken()
    {
        var bytes = new byte[32];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
