using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Dtos.Media;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Enums;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Identity;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionRepository : RepositoryBase<Contribution, Guid>, IContributionRepository
{
    private readonly AppDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ContributionRepository(AppDbContext context, IDateTimeProvider dateTimeProvider) : base(context)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<bool> IsSlugAlreadyExisted(string slug, Guid? contributionId = null)
    {
        if (contributionId is not null)
        {
            return await _context.Contributions.AnyAsync(x => x.Slug == slug && x.Id != contributionId);
        }

        return await _context.Contributions.AnyAsync(x => x.Slug == slug);
    }

    public async Task<PaginationResult<ContributionInListDto>> GetAllContributionsPagination(string? keyword, int pageIndex = 1, int pageSize = 10, string? academicYear = null, string? faculty = null, string? status = null)
    {
        var query = from c in _context.Contributions
                    where c.DateDeleted == null
                    join u in _context.Users on c.UserId equals u.Id
                    join f in _context.Faculties on c.FacultyId equals f.Id
                    join a in _context.AcademicYears on c.AcademicYearId equals a.Id
                    select new { c, u, f, a };

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.c.Title.Contains(keyword) || x.c.ShortDescription.Contains(keyword));
            //query = query.Where(x => EF.Functions.Like(x.c.Title, $"%{keyword}%") || EF.Functions.Like(x.c.ShortDescription, $"%{keyword}%"));
        }

        if (academicYear is not null)
        {
            query = query.Where(x => x.a.Name == academicYear);
        }

        if (faculty is not null)
        {
            query = query.Where(x => x.f.Name == faculty);
        }

        if (status is not null)
        {
            if (Enum.TryParse<ContributionStatus>(status.ToUpperInvariant(), true, out var requestStatus))
            {
                query = query.Where(x => x.c.Status == requestStatus);
            }
            else
            {
                throw new Exception("Invalid status param.");
            }
        }

        var totalRow = await query.CountAsync();

        pageIndex = pageIndex < 0 ? 1 : pageIndex;
        var skipPage = (pageIndex - 1) * pageSize;

        var contributions = await query
            .OrderByDescending(x => x.c.DateCreated)
            .Skip(skipPage)
            .Take(pageSize)
            .ToListAsync();

        // I think this is the only optimized approach here.
        var contributionIds = contributions.Select(x => x.c.Id).ToList();
        var files = await _context.Files.Where(f => contributionIds.Contains(f.ContributionId)).ToListAsync();

        var contributionsDto = contributions.Select(x => new ContributionInListDto
        {
            Id = x.c.Id,
            Title = x.c.Title,
            Slug = x.c.Slug,
            Thumbnails = files
                .Where(f => f.ContributionId == x.c.Id && f.Type == FileType.Thumbnail)
                .Select(f => new FileDto { Path = f.Path, Name = f.Name, Type = f.Type, PublicId = f.PublicId, Extension = f.Extension })
                .ToList(),
            Files = files
                .Where(f => f.ContributionId == x.c.Id && f.Type == FileType.File)
                .Select(f => new FileDto { Path = f.Path, Name = f.Name, Type = f.Type, PublicId = f.PublicId, Extension = f.Extension})
                .ToList(),
            Status = x.c.Status.ToString(),
            Username = x.u.UserName is not null ? x.u.UserName.ToString() : $"{x.u.FirstName} {x.u.LastName}",
            FacultyName = x.f.Name,
            AcademicYear = x.a.Name,
            SubmissionDate = x.c.DateCreated,
            PublicDate = x.c.PublicDate,
            ShortDescription = x.c.ShortDescription,
            RejectReason = null, // temporary
            GuestAllowed = x.c.AllowedGuest,
            Avatar = x.u.Avatar
        }).ToList();

        return new PaginationResult<ContributionInListDto>
        {
            CurrentPage = pageIndex,
            RowCount = totalRow,
            PageSize = pageSize,
            Results = contributionsDto
        };
    }

    public async Task ApproveContribution(Contribution contribution, Guid coordinatorId)
    {
        // who approve this contribution.
        var coordinator = await _context.Users.FirstOrDefaultAsync(x => x.Id == coordinatorId);

        if (coordinator is null)
        {
            throw new Exception("Coordinator was not found.");
        }

        contribution.Status = ContributionStatus.Approve;
        contribution.PublicDate = _dateTimeProvider.UtcNow;
        contribution.CoordinatorApprovedId = coordinator.Id;
        _context.Contributions.Update(contribution);
    }

    public async Task RejectContribution(Contribution contribution)
    {
        contribution.Status = ContributionStatus.Reject;
        _context.Contributions.Update(contribution);
    }
}
