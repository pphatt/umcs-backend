using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Dtos.Media;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Enums;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionPublicReadLaterRepository : RepositoryBase<ContributionPublicReadLater, Guid>, IContributionPublicReadLaterRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ContributionPublicReadLaterRepository(AppDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginationResult<PublicContributionInListDto>> GetAllReadLaterPublicContributionPagination(
        string? keyword,
        int pageIndex = 1,
        int pageSize = 10,
        Guid userId = default!,
        string? facultyName = null,
        string? academicYearName = null,
        string? orderBy = null)
    {
        // query all user save contribution
        var query = from rl in _context.ContributionPublicReadLaters
                    where rl.UserId == userId
                    join c in _context.ContributionPublics on rl.ContributionId equals c.Id
                    where c.DateDeleted == null
                    join u in _context.Users on c.UserId equals u.Id
                    join f in _context.Faculties on c.FacultyId equals f.Id
                    join a in _context.AcademicYears on c.AcademicYearId equals a.Id
                    select new { rl, c, u, f, a };

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
                        Enum.TryParse<ContributionOrderBy>(orderBy.ToUpperInvariant(), true, out var enumOrderBy) &&
                        enumOrderBy == ContributionOrderBy.Ascending;

        if (isAscending)
        {
            query = query.OrderBy(x => x.rl.DateCreated);
        }
        else
        {
            query = query.OrderByDescending(x => x.rl.DateCreated);
        }

        var rowCount = await query.CountAsync();

        pageIndex = pageIndex - 1 < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        var publicContributions = await query
            .Skip(skipPage)
            .Take(pageSize)
            .ToListAsync();

        // get all contribution pagination
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
            AlreadyLike = _context.Likes.AnyAsync(l => l.ContributionId == x.c.Id && l.UserId == x.rl.UserId).GetAwaiter().GetResult(),
            AlreadySaveReadLater = AlreadySave(x.c.Id, x.rl.UserId).GetAwaiter().GetResult(),
            AlreadyBookmark = _context.ContributionPublicBookmarks.AnyAsync(bm => bm.ContributionId == x.c.Id && bm.UserId == x.rl.UserId).GetAwaiter().GetResult(),
            WhoApproved = _context.Users.FindAsync(x.c.CoordinatorApprovedId).GetAwaiter().GetResult()!.UserName,
            Like = x.c.LikeQuantity,
            View = x.c.Views,
        }).ToList();

        // return that
        return new PaginationResult<PublicContributionInListDto>
        {
            CurrentPage = pageIndex,
            RowCount = rowCount,
            PageSize = pageSize,
            Results = result
        };
    }

    public async Task<bool> AlreadySave(Guid contributionId, Guid userId)
    {
        return await _context.ContributionPublicReadLaters.AnyAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }

    public async Task<ContributionPublicReadLater> GetSpecificSave(Guid contributionId, Guid userId)
    {
        return await _context.ContributionPublicReadLaters.FirstOrDefaultAsync(x => x.ContributionId == contributionId && x.UserId == userId);
    }
}
