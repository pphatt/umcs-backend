using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class PrivateChatMessageRepository : RepositoryBase<PrivateChatMessage, Guid>, IPrivateChatMessageRepository
{
    private readonly AppDbContext _context;

    public PrivateChatMessageRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
