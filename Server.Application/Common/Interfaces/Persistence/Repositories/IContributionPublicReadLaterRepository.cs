using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionPublicReadLaterRepository : IRepository<ContributionPublicReadLater, Guid>
{
    Task<bool> AlreadySave(Guid contributionId, Guid userId);

    Task<ContributionPublicReadLater> GetSpecificSave(Guid contributionId, Guid userId);
}
