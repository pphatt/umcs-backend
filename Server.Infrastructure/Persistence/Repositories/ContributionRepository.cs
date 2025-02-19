using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionRepository : RepositoryBase<Contribution, Guid>, IContributionRepository
{
    public ContributionRepository(AppDbContext context) : base(context)
    {
    }
}
