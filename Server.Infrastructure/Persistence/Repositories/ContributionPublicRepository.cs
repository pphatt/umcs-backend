using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Content.Like;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Dtos.Media;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Enums;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionPublicRepository : RepositoryBase<ContributionPublic, Guid>, IContributionPublicRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ContributionPublicRepository(AppDbContext context, IMapper mapper) : base(context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginationResult<PublicContributionInListDto>> GetAllPublicContributionsPagination(string? keyword,
        int pageIndex = 1,
        int pageSize = 10,
        string? academicYearName = null,
        string? facultyName = null,
        bool? allowedGuest = null,
        string? sortBy = null,
        string? orderBy = null)
    {
        var query = from c in _context.ContributionPublics
                    where c.DateDeleted == null
                    join u in _context.Users on c.UserId equals u.Id
                    join f in _context.Faculties on c.FacultyId equals f.Id
                    join a in _context.AcademicYears on c.AcademicYearId equals a.Id
                    select new { c, u, f, a };

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.c.Title.Contains(keyword) ||
                                     x.c.Content.Contains(keyword) ||
                                     x.c.ShortDescription.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(academicYearName))
        {
            query = query.Where(x => x.a.Name == academicYearName);
        }

        if (!string.IsNullOrWhiteSpace(facultyName))
        {
            query = query.Where(x => x.f.Name == facultyName);
        }

        if (allowedGuest is not null)
        {
            query = query.Where(x => x.c.AllowedGuest == allowedGuest);
        }

        bool isAscending = !string.IsNullOrWhiteSpace(orderBy) &&
                           Enum.TryParse<ContributionOrderBy>(orderBy.ToUpperInvariant(), true, out var enumOrder) &&
                           enumOrder == ContributionOrderBy.Ascending;

        if (string.IsNullOrWhiteSpace(sortBy))
        {
            query = (isAscending) switch
            {
                (true) => query.OrderBy(x => x.c.PublicDate),
                _ => query.OrderByDescending(x => x.c.PublicDate),
            };
        }
        else if (Enum.TryParse<ContributionSortBy>(sortBy.ToUpperInvariant(), true, out var enumSort))
        {
            query = (enumSort, isAscending) switch
            {
                (ContributionSortBy.PublicDate, true) => query.OrderBy(x => x.c.PublicDate),
                (ContributionSortBy.PublicDate, false) => query.OrderByDescending(x => x.c.PublicDate),
                (ContributionSortBy.Like, true) => query.OrderBy(x => x.c.LikeQuantity),
                (ContributionSortBy.Like, false) => query.OrderByDescending(x => x.c.LikeQuantity),
                (ContributionSortBy.View, true) => query.OrderBy(x => x.c.Views),
                (ContributionSortBy.View, false) => query.OrderByDescending(x => x.c.Views),
                _ => query.OrderByDescending(x => x.c.PublicDate)
            };
        }
        else
        {
            query = query.OrderByDescending(x => x.c.PublicDate);
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

    public async Task<PaginationResult<ContributorDto>> GetTopContributors(string? keyword,
        int pageIndex = 1,
        int pageSize = 10,
        string? facultyName = null,
        string? orderBy = null)
    {
        var query = from c in _context.ContributionPublics
                    where c.DateDeleted == null
                    join f in _context.Faculties on c.FacultyId equals f.Id
                    group new { c, f } by new { c.UserId, f.Name, c.Avatar } into userGroup
                    select new {
                        Username = userGroup.FirstOrDefault().c.Username,
                        FacultyName = userGroup.FirstOrDefault().c.FacultyName,
                        Avatar = userGroup.FirstOrDefault().c.Avatar,
                        TotalLikes = userGroup.Sum(x => x.c.LikeQuantity),
                        TotalContributions = userGroup.Count()
                    };

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Username.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(facultyName))
        {
            query = query.Where(x => x.FacultyName == facultyName);
        }

        bool isAscending = !string.IsNullOrWhiteSpace(orderBy) &&
                          Enum.TryParse<ContributionOrderBy>(orderBy.ToUpperInvariant(), true, out var enumOrder) &&
                          enumOrder == ContributionOrderBy.Ascending;

        if (isAscending)
        {
            query = query
                .OrderBy(x => x.TotalLikes)
                .ThenBy(x => x.TotalContributions);
        }
        else
        {
            query = query
                .OrderByDescending(x => x.TotalLikes)
                .ThenByDescending(x => x.TotalContributions);
        }

        var rowCount = await query.CountAsync();

        pageIndex = pageIndex - 1 < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        var result = await query
            .Skip(skipPage)
            .Take(pageSize)
            .Select(x => new ContributorDto
            {
                Username = x.Username,
                FacultyName = x.FacultyName,
                Avatar = x.Avatar,
                TotalLikes = x.TotalLikes,
                TotalContributions = x.TotalContributions
            })
            .ToListAsync();

        //List<ContributorDto> result = _mapper.Map<List<ContributorDto>>(query);

        //var contributions = await _context.ContributionPublics
        //    .Where(x => x.DateDeleted == null)
        //    .Select(x => new
        //    {
        //        x.Username,
        //        x.Avatar,
        //        x.LikeQuantity,
        //        x.PublicDate
        //    })
        //    .ToListAsync();

        //var result = contributions
        //    .GroupBy(x => x.Username)
        //    .Select(group => new
        //    {
        //        UserName = group.Key,
        //        Avatar = group.FirstOrDefault().Avatar,
        //        TotalLikes = group.Sum(c => c.LikeQuantity),
        //        ContributionCount = group.Count()
        //    })
        //    .OrderByDescending(x => x.TotalLikes)
        //    .ThenByDescending(x => x.ContributionCount)
        //    .Take(pageSize)
        //    .Select(x => new ContributorDto
        //    {
        //        Username = x.UserName,
        //        Avatar = x.Avatar,
        //        TotalLikes = x.TotalLikes,
        //        TotalPublicContributionCount = x.ContributionCount
        //    });

        return new PaginationResult<ContributorDto>
        {
            CurrentPage = pageIndex,
            RowCount = rowCount,
            PageSize = pageSize,
            Results = result
        };
    }

    public async Task<PublicContributionDto> GetPublicContributionBySlug(string slug)
    {
        var query = from c in _context.ContributionPublics
                    where c.DateDeleted == null && c.Slug == slug
                    join u in _context.Users on c.UserId equals u.Id
                    join f in _context.Faculties on c.FacultyId equals f.Id
                    join a in _context.AcademicYears on c.AcademicYearId equals a.Id
                    select new { c, u, f, a };

        var contribution = await query.FirstOrDefaultAsync();

        if (contribution is null)
        {
            return null;
        }

        var files = await _context.Files.Where(x => x.ContributionId == contribution.c.Id).ToListAsync();

        var result = new PublicContributionDto
        {
            Id = contribution.c.Id,
            Title = contribution.c.Title,
            Slug = contribution.c.Slug,
            Content = contribution.c.Content,
            ShortDescription = contribution.c.ShortDescription,
            Username = contribution.u.UserName is not null ? contribution.u.UserName.ToString() : $"{contribution.u.FirstName} {contribution.u.LastName}",
            FacultyName = contribution.f.Name,
            AcademicYearName = contribution.a.Name,
            Thumbnails = files
                .Where(f => f.ContributionId == contribution.c.Id && f.Type == FileType.Thumbnail)
                .Select(f => new FileDto { Path = f.Path, Name = f.Name, Type = f.Type, PublicId = f.PublicId, Extension = f.Extension })
                .ToList(),
            Files = files
                .Where(f => f.ContributionId == contribution.c.Id && f.Type == FileType.File)
                .Select(f => new FileDto { Path = f.Path, Name = f.Name, Type = f.Type, PublicId = f.PublicId, Extension = f.Extension })
                .ToList(),
            PublicDate = contribution.c.PublicDate,
            SubmissionDate = contribution.c.SubmissionDate,
            DateEdited = contribution.c.DateUpdated,
            Avatar = contribution.u.Avatar,
            WhoApproved = _context.Users.FindAsync(contribution.c.CoordinatorApprovedId).GetAwaiter().GetResult()!.UserName,
            Like = contribution.c.LikeQuantity,
            View = contribution.c.Views,
            AllowedGuest = contribution.c.AllowedGuest,
        };

        return result;
    }

    public async Task<PaginationResult<UserLikeInListDto>> GetAllUsersLikedContributionPagination(Guid contributionId, int pageIndex = 1, int pageSize = 10)
    {
        var query = from l in _context.Likes
                    where l.ContributionId == contributionId
                    join u in _context.Users on l.UserId equals u.Id
                    select new { l, u };

        var rowCount = await query.CountAsync();

        pageIndex = pageIndex - 1 < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        var users = await query
            .OrderByDescending(x => x.l.DateCreated)
            .Skip(skipPage)
            .Take(pageSize)
            .Select(x => new UserLikeInListDto
            {
                Username = x.u.UserName,
                Email = x.u.Email,
                Avatar = x.u.Avatar,
                DateCreated = x.l.DateCreated
            })
            .ToListAsync();

        return new PaginationResult<UserLikeInListDto>
        {
            CurrentPage = pageIndex,
            RowCount = rowCount,
            PageSize = pageSize,
            Results = users
        };
    }
}
