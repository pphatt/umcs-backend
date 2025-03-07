using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionPublicBookmarkRepository : IRepository<ContributionPublicBookmark, Guid>
{
    Task<bool> AlreadyBookmark(Guid contributionId, Guid userId);

    Task<ContributionPublicBookmark> GetSpecificBookmark(Guid contributionId, Guid userId);
}
