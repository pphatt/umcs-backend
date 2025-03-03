using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionCommentRepository : RepositoryBase<ContributionComment, Guid>, IContributionCommentRepository
{
    private readonly AppDbContext _context;

    public ContributionCommentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
