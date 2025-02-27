using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionPublicRepository : RepositoryBase<ContributionPublic, Guid>, IContributionPublicRepository
{
    private readonly AppDbContext _context;

    public ContributionPublicRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
