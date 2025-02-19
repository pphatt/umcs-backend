using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionRepository : RepositoryBase<Contribution, Guid>, IContributionRepository
{
    private readonly AppDbContext _context;

    public ContributionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsSlugAlreadyExisted(string slug, Guid? contributionId = null)
    {
        if (contributionId is not null)
        {
            return await _context.Contributions.AnyAsync(x => x.Slug == slug && x.Id != contributionId);
        }

        return await _context.Contributions.AnyAsync(x => x.Slug == slug);
    }
}
