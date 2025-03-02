using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class LikeRepository : RepositoryBase<Like, Guid>, ILikeRepository
{
    private readonly AppDbContext _context;

    public LikeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> AlreadyLike(Guid contributionId, Guid userId)
    {
        return await _context.Likes.AnyAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }

    public async Task<Like> GetSpecificLike(Guid contributionId, Guid userId)
    {
        return await _context.Likes.FirstOrDefaultAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }
}
