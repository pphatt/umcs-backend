using Server.Domain.Entity.Identity;

namespace Server.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    Task<string> GenerateToken(AppUser user);
}
