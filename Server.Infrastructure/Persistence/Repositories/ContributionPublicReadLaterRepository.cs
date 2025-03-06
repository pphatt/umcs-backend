using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionPublicReadLaterRepository : RepositoryBase<ContributionPublicReadLater, Guid>, IContributionPublicReadLaterRepository
{
    private readonly AppDbContext _context;

    public ContributionPublicReadLaterRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> AlreadySave(Guid contributionId, Guid userId)
    {
        return await _context.ContributionPublicReadLaters.AnyAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }

    public async Task<ContributionPublicReadLater> GetSpecificSave(Guid contributionId, Guid userId)
    {
        return await _context.ContributionPublicReadLaters.FirstOrDefaultAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }
}
