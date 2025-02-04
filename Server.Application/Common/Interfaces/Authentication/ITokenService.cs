namespace Server.Application.Common.Interfaces.Authentication;

public interface ITokenService
{
    string GenerateRefreshToken();
    Task StoreRefreshTokenAsync(Guid userId, string refreshToken);
}
