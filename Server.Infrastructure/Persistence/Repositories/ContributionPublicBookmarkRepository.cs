using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionPublicBookmarkRepository : RepositoryBase<ContributionPublicBookmark, Guid>, IContributionPublicBookmarkRepository
{
    private readonly AppDbContext _context;

    public ContributionPublicBookmarkRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> AlreadyBookmark(Guid contributionId, Guid userId)
    {
        return await _context.ContributionPublicBookmarks.AnyAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }

    public async Task<ContributionPublicBookmark> GetSpecificBookmark(Guid contributionId, Guid userId)
    {
        return await _context.ContributionPublicBookmarks.FirstOrDefaultAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }
}
