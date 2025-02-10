using Server.Domain.Entity.Token;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface ITokenRepository : IRepository<RefreshToken, Guid>
{
}
