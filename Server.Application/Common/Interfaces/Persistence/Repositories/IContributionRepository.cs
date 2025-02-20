using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionRepository : IRepository<Contribution, Guid>
{
    Task<bool> IsSlugAlreadyExisted(string slug, Guid? contributionId = null);
}
