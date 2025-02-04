namespace Server.Application.Common.Interfaces.Authentication;

public interface ITokenService
{
    string GenerateRefreshToken();
}
