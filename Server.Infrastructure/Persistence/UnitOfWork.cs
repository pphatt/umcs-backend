using AutoMapper;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Common.Interfaces.Services;
using Server.Infrastructure.Persistence.Repositories;

namespace Server.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UnitOfWork(AppDbContext context, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public ITokenRepository TokenRepository => new TokenRepository(_context);

    public IFacultyRepository FacultyRepository => new FacultyRepository(_context, _mapper);

    public IAcademicYearRepository AcademicYearRepository => new AcademicYearRepository(_context, _mapper);

    public IContributionRepository ContributionRepository => new ContributionRepository(_context, _dateTimeProvider);

    public IFileRepository FileRepository => new FileRepository(_context);

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
