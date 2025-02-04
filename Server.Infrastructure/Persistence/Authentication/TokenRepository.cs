using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Persistence.Authentication;
using Server.Domain.Entity.Token;

namespace Server.Infrastructure.Persistence.Authentication;

public class TokenRepository : RepositoryBase<RefreshToken, Guid>, ITokenRepository
{
    private readonly AppDbContext _context;

    public TokenRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
