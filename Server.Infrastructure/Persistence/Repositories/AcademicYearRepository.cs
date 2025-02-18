using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class AcademicYearRepository : RepositoryBase<AcademicYear, Guid>, IAcademicYearRepository
{
    private readonly AppDbContext _context;

    public AcademicYearRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<AcademicYear> GetAcademicYearByNameAsync(string name)
    {
        return await _context.AcademicYears.SingleOrDefaultAsync(x => x.Name == name);
    }

    public async Task<bool> HasContributionsAsync(Guid academicYearId)
    {
        return await _context.Contributions.AnyAsync(x => x.AcademicYearId == academicYearId);
    }
}
