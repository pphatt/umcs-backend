using Server.Domain.Entity.Token;

namespace Server.Application.Common.Interfaces.Authentication;

public interface ITokenService
{
    string GenerateRefreshToken();
    Task StoreRefreshTokenAsync(Guid userId, string refreshToken);
    RefreshToken? GetByTokenAsync(string token);
    Task UpdateRefreshTokenAsync(RefreshToken newRefreshToken);
}
