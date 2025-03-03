using Server.Application.Common.Dtos.Content.Comment;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionCommentRepository : IRepository<ContributionComment, Guid>
{
    Task<List<CommentDto>> GetCommentsByContributionId(Guid contributionId);
}
