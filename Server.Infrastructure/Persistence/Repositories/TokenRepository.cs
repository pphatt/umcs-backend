using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Token;

namespace Server.Infrastructure.Persistence.Repositories;

public class TokenRepository : RepositoryBase<RefreshToken, Guid>, ITokenRepository
{
    private readonly AppDbContext _context;

    public TokenRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
