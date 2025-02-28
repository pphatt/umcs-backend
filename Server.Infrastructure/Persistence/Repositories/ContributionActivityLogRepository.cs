using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionActivityLogRepository : RepositoryBase<ContributionActivityLog, Guid>, IContributionActivityLogRepository
{
    private readonly AppDbContext _context;

    public ContributionActivityLogRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
