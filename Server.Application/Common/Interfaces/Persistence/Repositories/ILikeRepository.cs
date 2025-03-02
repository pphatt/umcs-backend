using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface ILikeRepository : IRepository<Like, Guid>
{
    Task<bool> AlreadyLike(Guid contributionId, Guid userId);

    Task<Like> GetSpecificLike(Guid contributionId, Guid userId);
}
