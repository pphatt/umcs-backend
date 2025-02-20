using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class FacultyRepository : RepositoryBase<Faculty, Guid>, IFacultyRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public FacultyRepository(AppDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Count()
    {
        return await _context.Faculties.CountAsync();
    }

    public async Task<PaginationResult<FacultyDto>> GetAllFacultiesPagination(string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        var query = _context.Faculties.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query
                .Where(x => x.Name.Contains(keyword));
        }

        var count = await query.CountAsync();

        pageIndex = pageIndex < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        query = query
            .Where(x => x.DateDeleted == null)
            .Skip(skipPage)
            .Take(pageSize)
            .OrderByDescending(x => x.DateCreated);

        var result = await _mapper.ProjectTo<FacultyDto>(query).ToListAsync();

        return new PaginationResult<FacultyDto>
        {
            CurrentPage = pageIndex,
            RowCount = count,
            PageSize = pageSize,
            Results = result
        };
    }

    public async Task<Faculty> GetFacultyByNameAsync(string name)
    {
        return await _context.Faculties.SingleOrDefaultAsync(x => x.Name == name);
    }
}
