using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Dtos.Media;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Enums;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionPublicBookmarkRepository : RepositoryBase<ContributionPublicBookmark, Guid>, IContributionPublicBookmarkRepository
{
    private readonly AppDbContext _context;

    public ContributionPublicBookmarkRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PaginationResult<PublicContributionInListDto>> GetAllBookmarkPagination(string? keyword,
        int pageIndex = 1,
        int pageSize = 10,
        Guid userId = default!,
        string? facultyName = null,
        string? academicYearName = null,
        string? orderBy = null
    )
    {
        var query = from bm in _context.ContributionPublicBookmarks
                    where bm.UserId == userId
                    join c in _context.ContributionPublics on bm.ContributionId equals c.Id
                    where c.DateDeleted == null
                    join u in _context.Users on bm.UserId equals u.Id
                    join f in _context.Faculties on c.FacultyId equals f.Id
                    join a in _context.AcademicYears on c.AcademicYearId equals a.Id
                    select new { bm, c, u, f, a };

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.c.Title.Contains(keyword) ||
                                     x.c.Content.Contains(keyword) ||
                                     x.c.ShortDescription.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(facultyName))
        {
            query = query.Where(x => x.f.Name == facultyName);
        }

        if (!string.IsNullOrWhiteSpace(academicYearName))
        {
            query = query.Where(x => x.a.Name == academicYearName);
        }

        var isAscending = !string.IsNullOrWhiteSpace(orderBy) &&
                          Enum.TryParse<OrderByEnum>(orderBy.ToUpperInvariant(), true, out var enumOrderBy) &&
                          enumOrderBy == OrderByEnum.Ascending;

        if (isAscending)
        {
            query = query.OrderBy(x => x.bm.DateCreated);
        }
        else
        {
            query = query.OrderByDescending(x => x.bm.DateCreated);
        }

        var rowCount = await query.CountAsync();

        pageIndex = pageIndex - 1 < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        var publicContributions = await query
            .Skip(skipPage)
            .Take(pageSize)
            .ToListAsync();

        var contributionIds = publicContributions.Select(x => x.c.Id).ToList();

        var files = await _context.Files.Where(x => contributionIds.Contains(x.ContributionId)).ToListAsync();

        var result = publicContributions.Select(x => new PublicContributionInListDto
        {
            Id = x.c.Id,
            Title = x.c.Title,
            ShortDescription = x.c.ShortDescription,
            Slug = x.c.Slug,
            Username = x.u.UserName is not null ? x.u.UserName.ToString() : $"{x.u.FirstName} {x.u.LastName}",
            FacultyName = x.f.Name,
            AcademicYearName = x.a.Name,
            Thumbnails = files
                .Where(f => f.ContributionId == x.c.Id && f.Type == FileType.Thumbnail)
                .Select(f => new FileDto { Path = f.Path, Name = f.Name, Type = f.Type, PublicId = f.PublicId, Extension = f.Extension })
                .ToList(),
            PublicDate = x.c.PublicDate,
            SubmissionDate = x.c.SubmissionDate,
            DateEdited = x.c.DateUpdated,
            Avatar = x.u.Avatar,
            GuestAllowed = x.c.AllowedGuest,
            AlreadyLike = _context.Likes.AnyAsync(l => l.ContributionId == x.c.Id && l.UserId == x.bm.UserId).GetAwaiter().GetResult(),
            AlreadySaveReadLater = _context.ContributionPublicReadLaters.AnyAsync(rl => rl.ContributionId == x.c.Id && rl.UserId == x.bm.UserId).GetAwaiter().GetResult(),
            AlreadyBookmark = AlreadyBookmark(x.c.Id, x.bm.UserId).GetAwaiter().GetResult(),
            WhoApproved = _context.Users.FindAsync(x.c.CoordinatorApprovedId).GetAwaiter().GetResult()!.UserName,
            Like = x.c.LikeQuantity,
            View = x.c.Views,
        }).ToList();

        return new PaginationResult<PublicContributionInListDto>
        {
            CurrentPage = pageIndex,
            RowCount = rowCount,
            PageSize = pageSize,
            Results = result
        };
    }

    public async Task<bool> AlreadyBookmark(Guid contributionId, Guid userId)
    {
        return await _context.ContributionPublicBookmarks.AnyAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }

    public async Task<ContributionPublicBookmark> GetSpecificBookmark(Guid contributionId, Guid userId)
    {
        return await _context.ContributionPublicBookmarks.FirstOrDefaultAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }
}
