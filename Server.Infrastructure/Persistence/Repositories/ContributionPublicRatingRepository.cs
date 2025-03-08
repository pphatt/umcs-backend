using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionPublicRatingRepository : RepositoryBase<ContributionPublicRating, Guid>, IContributionPublicRatingRepository
{
    private readonly AppDbContext _context;

    public ContributionPublicRatingRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> AlreadyRate(Guid contributionId, Guid userId)
    {
        return await _context.ContributionPublicRatings.AnyAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }

    public async Task<ContributionPublicRating> GetSpecificRating(Guid contributionId, Guid userId)
    {
        return await _context.ContributionPublicRatings.FirstOrDefaultAsync(x => x.ContributionId == contributionId && userId == userId);
    }

    public async Task<double> GetContributionAverageRating(Guid contributionId)
    {
        var ratings = await _context.ContributionPublicRatings
            .Where(x => x.ContributionId == contributionId).ToListAsync();

        return ratings.Count() == 0 ? 0.0 : ratings.Average(x => x.Rating);
    }
}
