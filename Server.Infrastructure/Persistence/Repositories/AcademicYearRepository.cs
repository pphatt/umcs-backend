using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class AcademicYearRepository : RepositoryBase<AcademicYear, Guid>, IAcademicYearRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AcademicYearRepository(AppDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AcademicYear> GetAcademicYearByNameAsync(string name)
    {
        return await _context.AcademicYears.SingleOrDefaultAsync(x => x.Name == name);
    }

    public async Task<bool> HasContributionsAsync(Guid academicYearId)
    {
        return await _context.Contributions.AnyAsync(x => x.AcademicYearId == academicYearId);
    }

    public async Task<PaginationResult<AcademicYearDto>> GetAllAcademicYearsPagination(string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        var query = _context.AcademicYears.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query
                .Where(x => x.Name.Contains(keyword));
        }

        var count = await query.CountAsync();

        pageIndex = pageIndex - 1 < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        query = query
            .Where(x => x.DateDeleted == null)
            .Take(pageSize)
            .Skip(skipPage)
            .OrderByDescending(x => x.DateCreated);

        var result = await _mapper.ProjectTo<AcademicYearDto>(query).ToListAsync();

        return new PaginationResult<AcademicYearDto>
        {
            CurrentPage = pageIndex,
            RowCount = count,
            PageSize = pageSize,
            Results = result
        };
    }
}
