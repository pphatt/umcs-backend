using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class FacultyRepository : RepositoryBase<Faculty, Guid>, IFacultyRepository
{
    private readonly AppDbContext _context;

    public FacultyRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
