using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionPublicCommentRepository : RepositoryBase<ContributionPublicComment, Guid>, IContributionPublicCommentRepository
{
    private readonly AppDbContext _context;

    public ContributionPublicCommentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
