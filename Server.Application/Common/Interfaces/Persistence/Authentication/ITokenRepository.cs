using Server.Domain.Entity.Token;

namespace Server.Application.Common.Interfaces.Persistence.Authentication;

public interface ITokenRepository : IRepository<RefreshToken, Guid>
{
}
