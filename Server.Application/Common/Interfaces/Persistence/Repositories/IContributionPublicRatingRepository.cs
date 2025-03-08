using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionPublicRatingRepository : IRepository<ContributionPublicRating, Guid>
{
    Task<bool> AlreadyRate(Guid contributionId, Guid userId);

    Task<ContributionPublicRating> GetSpecificRating(Guid contributionId, Guid userId);

    Task<double> GetContributionAverageRating(Guid contributionId);
}
