using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Persistence.Authentication;
using Server.Infrastructure.Persistence.Authentication;

namespace Server.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ITokenRepository TokenRepository => new TokenRepository(_context);

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
