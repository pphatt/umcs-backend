using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Contracts.Common.Media;
using File = Server.Domain.Entity.Content.File;

namespace Server.Infrastructure.Persistence.Repositories;

public class FileRepository : RepositoryBase<File, Guid>, IFileRepository
{
    private readonly AppDbContext _context;

    public FileRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<File>> GetByContributionIdAsync(Guid contributionId)
    {
        var files = await _context.Files.Where(x => x.ContributionId == contributionId).ToListAsync();

        return files;
    }
}
