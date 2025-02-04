using Server.Application.Common.Interfaces.Persistence.Authentication;

namespace Server.Application.Common.Interfaces.Persistence;

public interface IUnitOfWork
{
    Task<int> CompleteAsync();

    ITokenRepository TokenRepository { get; }
}
