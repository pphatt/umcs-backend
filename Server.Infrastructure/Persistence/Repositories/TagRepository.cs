using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Content.Tag;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class TagRepository : RepositoryBase<Tag, Guid>, ITagRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TagRepository(AppDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginationResult<TagInListDto>> GetAllTagsPagination(string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        var query = _context.Tags
            .Where(x => x.DateDeleted == null)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword));
        }

        var rowCount = await query.CountAsync();

        pageIndex = pageIndex - 1 < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        query = query
            .OrderByDescending(x => x.DateCreated)
            .Skip(skipPage)
            .Take(pageSize);

        var result = await _mapper.ProjectTo<TagInListDto>(query).ToListAsync();

        return new PaginationResult<TagInListDto>
        {
            CurrentPage = pageIndex,
            RowCount = rowCount,
            PageSize = pageSize,
            Results = result
        };
    }

    public async Task<Tag> GetTagByName(string name)
    {
        return await _context.Tags.FirstOrDefaultAsync(x => x.Name == name);
    }
}
