using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Common.Constants.Content;

namespace Server.Infrastructure.Persistence.Repositories;

using File = Domain.Entity.Content.File;

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

    public async Task<List<File>> GetByListContributionIdsAsync(List<Guid> contributionIds)
    {
        var files = await _context.Files.Where(x => contributionIds.Contains(x.ContributionId)).ToListAsync();

        return files;
    }

    public async Task<List<string>> GetFilesPathByContributionId(Guid contributionId)
    {
        var paths = await _context.Files.Where(x => x.ContributionId == contributionId && x.Type == FileType.File).Select(x => x.PublicId).ToListAsync();

        return paths;
    }
}
