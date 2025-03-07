using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Dtos.Media;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Enums;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class LikeRepository : RepositoryBase<Like, Guid>, ILikeRepository
{
    private readonly AppDbContext _context;

    public LikeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PaginationResult<PublicContributionInListDto>> GetAllUserLikePublicContributionsPagination(string? keyword = null,
        int pageIndex = 1,
        int pageSize = 10,
        Guid userId = default!,
        string? facultyName = null,
        string? academicYearName = null,
        string? orderBy = null)
    {
        var query = from l in _context.Likes
                    where l.UserId == userId
                    join c in _context.ContributionPublics on l.ContributionId equals c.Id
                    where c.DateDeleted == null
                    join u in _context.Users on l.UserId equals u.Id
                    join f in _context.Faculties on c.FacultyId equals f.Id
                    join a in _context.AcademicYears on c.AcademicYearId equals a.Id
                    select new { l, c, u, f, a };

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
                         Enum.TryParse<ContributionOrderBy>(orderBy, true, out var enumOrderBy) &&
                         enumOrderBy == ContributionOrderBy.Ascending;

        if (isAscending)
        {
            query = query.OrderBy(x => x.l.DateCreated);
        }
        else
        {
            query = query.OrderByDescending(x => x.l.DateCreated);
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
            AlreadyLike = AlreadyLike(x.c.Id, x.l.UserId).GetAwaiter().GetResult(),
            AlreadySaveReadLater = _context.ContributionPublicReadLaters.AnyAsync(rl => rl.ContributionId == x.c.Id && rl.UserId == userId).GetAwaiter().GetResult(),
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

    public async Task<bool> AlreadyLike(Guid contributionId, Guid userId)
    {
        return await _context.Likes.AnyAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }

    public async Task<Like> GetSpecificLike(Guid contributionId, Guid userId)
    {
        return await _context.Likes.FirstOrDefaultAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }
}
