using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class TagRepository : RepositoryBase<Tag, Guid>, ITagRepository
{
    private readonly AppDbContext _context;

    public TagRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Tag> GetTagByName(string name)
    {
        return await _context.Tags.FirstOrDefaultAsync(x => x.Name == name);
    }
}
