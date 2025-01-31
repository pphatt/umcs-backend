using Server.Domain.Entity.Identity;

namespace Server.Domain.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(AppUser user);
}
