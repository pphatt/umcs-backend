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

    public IContributionRepository ContributionRepository => new ContributionRepository(_context, _mapper, _dateTimeProvider, FileRepository);

    public IContributionPublicRepository ContributionPublicRepository => new ContributionPublicRepository(_context, _mapper, LikeRepository, ContributionPublicReadLaterRepository);

    public IContributionActivityLogRepository ContributionActivityLogRepository => new ContributionActivityLogRepository(_context, _mapper, FacultyRepository, AcademicYearRepository);

    public IContributionCommentRepository ContributionCommentRepository => new ContributionCommentRepository(_context);

    public IContributionPublicCommentRepository ContributionPublicCommentRepository => new ContributionPublicCommentRepository(_context);

    public IContributionPublicReadLaterRepository ContributionPublicReadLaterRepository => new ContributionPublicReadLaterRepository(_context, _mapper);

    public IContributionPublicBookmarkRepository ContributionPublicBookmarkRepository => new ContributionPublicBookmarkRepository(_context);

    public IFileRepository FileRepository => new FileRepository(_context);

    public ILikeRepository LikeRepository => new LikeRepository(_context);

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
