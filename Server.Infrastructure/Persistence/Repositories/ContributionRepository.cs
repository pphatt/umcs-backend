using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Common.Dtos.Media;
using Server.Application.Common.Extensions;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Common.Interfaces.Services;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Constants.Content;
using Server.Domain.Common.Enums;
using Server.Domain.Entity.Content;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionRepository : RepositoryBase<Contribution, Guid>, IContributionRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IFileRepository _fileRepository;

    public ContributionRepository(AppDbContext context, IMapper mapper, IDateTimeProvider dateTimeProvider, IFileRepository fileRepository) : base(context)
    {
        _context = context;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _fileRepository = fileRepository;
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
        var files = await _fileRepository.GetByListContributionIdsAsync(contributionIds);

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
            Status = x.c.Status.ToStringValue(),
            Username = x.u.UserName is not null ? x.u.UserName.ToString() : $"{x.u.FirstName} {x.u.LastName}",
            FacultyName = x.f.Name,
            AcademicYear = x.a.Name,
            SubmissionDate = x.c.DateCreated,
            PublicDate = x.c.PublicDate,
            ShortDescription = x.c.ShortDescription,
            RejectReason = GetRejectionReason(x.c).GetAwaiter().GetResult(),
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

    public async Task<ContributionDto> GetContributionBySlugAndFaculty(string slug, Guid facultyId)
    {
        var query = from c in _context.Contributions
                    where c.Slug == slug && c.DateDeleted == null
                    join u in _context.Users on c.UserId equals u.Id
                    join f in _context.Faculties on c.FacultyId equals f.Id
                    join a in _context.AcademicYears on c.AcademicYearId equals a.Id
                    where f.Id == facultyId
                    select new { c, u, f, a };

        var contribution = await query.FirstOrDefaultAsync();

        if (contribution is null)
        {
            return null;
        }

        var files = await _fileRepository.GetByContributionIdAsync(contribution.c.Id);

        var result = new ContributionDto
        {
            Id = contribution.c.Id,
            Title = contribution.c.Title,
            Slug = contribution.c.Slug,
            Thumbnails = files
                .Where(f => f.ContributionId == contribution.c.Id && f.Type == FileType.Thumbnail)
                .Select(f => new FileDto { Path = f.Path, Name = f.Name, Type = f.Type, PublicId = f.PublicId, Extension = f.Extension })
                .ToList(),
            Files = files
                .Where(f => f.ContributionId == contribution.c.Id && f.Type == FileType.File)
                .Select(f => new FileDto { Path = f.Path, Name = f.Name, Type = f.Type, PublicId = f.PublicId, Extension = f.Extension })
                .ToList(),
            Status = contribution.c.Status.ToStringValue(),
            Username = contribution.u.UserName is not null ? contribution.u.UserName.ToString() : $"{contribution.u.FirstName} {contribution.u.LastName}",
            FacultyName = contribution.f.Name,
            AcademicYear = contribution.a.Name,
            SubmissionDate = contribution.c.DateCreated,
            PublicDate = contribution.c.PublicDate,
            ShortDescription = contribution.c.ShortDescription,
            GuestAllowed = contribution.c.AllowedGuest,
            Avatar = contribution.u.Avatar
        };

        return result;
    }

    public async Task<ContributionDto> GetPersonalContributionBySlug(string slug, Guid userId)
    {
        var query = from c in _context.Contributions
                    where c.Slug == slug && c.DateDeleted == null && c.UserId == userId
                    join u in _context.Users on c.UserId equals u.Id
                    join f in _context.Faculties on c.FacultyId equals f.Id
                    join a in _context.AcademicYears on c.AcademicYearId equals a.Id
                    select new { c, u, f, a };

        var contribution = await query.FirstOrDefaultAsync();

        if (contribution is null)
        {
            return null;
        }

        var files = await _fileRepository.GetByContributionIdAsync(contribution.c.Id);

        var result = new ContributionDto
        {
            Id = contribution.c.Id,
            Title = contribution.c.Title,
            Slug = contribution.c.Slug,
            Content = contribution.c.Content,
            ShortDescription = contribution.c.ShortDescription,
            Username = contribution.u.UserName is not null ? contribution.u.UserName.ToString() : $"{contribution.u.FirstName} {contribution.u.LastName}",
            FacultyName = contribution.f.Name,
            AcademicYear = contribution.a.Name,
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
            DateUpdated = contribution.c.DateUpdated,
            Avatar = contribution.u.Avatar,
            Status = contribution.c.Status.ToStringValue(),
        };

        return result;
    }

    public async Task SendToApproved(Guid contributionId, Guid studentId)
    {
        var contribution = await _context.Contributions.FindAsync(contributionId);

        if (contribution is null)
        {
            throw new Exception($"Contribution with id of {contributionId} was not found.");
        }

        var student = await _context.Users.FindAsync(studentId);

        if (student is null)
        {
            throw new Exception("Student was not found.");
        }

        await _context.ContributionActivityLogs.AddAsync(new ContributionActivityLog
        {
            ContributionId = contribution.Id,
            ContributionTitle = contribution.Title,
            UserId = student.Id,
            Username = student.UserName,
            FromStatus = contribution.Status,
            ToStatus = ContributionStatus.Pending,
            Description = $"{student.UserName} submit new contribution and wait for approval."
        });
    }

    public async Task ApproveContribution(Contribution contribution, Guid coordinatorId)
    {
        // who approve this contribution.
        var coordinator = await _context.Users.FirstOrDefaultAsync(x => x.Id == coordinatorId);

        if (coordinator is null)
        {
            throw new Exception("Current authenticated user was not found.");
        }

        var student = await _context.Users.FindAsync(contribution.UserId);

        if (student is null)
        {
            throw new Exception("Contribution's owner is not found.");
        }

        var faculty = await _context.Faculties.FindAsync(contribution.FacultyId);

        if (faculty is null)
        {
            throw new Exception("Contribution's facultyName is not found.");
        }

        await _context.ContributionActivityLogs.AddAsync(new ContributionActivityLog
        {
            ContributionId = contribution.Id,
            ContributionTitle = contribution.Title,
            UserId = coordinator.Id,
            Username = coordinator.UserName,
            FromStatus = contribution.Status,
            ToStatus = ContributionStatus.Approve,
            Description = $"{coordinator.UserName} approve",
        });

        contribution.Status = ContributionStatus.Approve;
        contribution.PublicDate = _dateTimeProvider.UtcNow;
        contribution.CoordinatorApprovedId = coordinator.Id;
        _context.Contributions.Update(contribution);

        var publicContribution = _mapper.Map<ContributionPublic>(contribution);

        publicContribution.Id = contribution.Id;
        publicContribution.Username = student.UserName ?? string.Empty;
        publicContribution.Avatar = student.Avatar ?? string.Empty;
        publicContribution.FacultyName = faculty.Name ?? string.Empty;
        publicContribution.DateCreated = _dateTimeProvider.UtcNow;

        await _context.ContributionPublics.AddAsync(publicContribution);
    }

    public async Task RejectContribution(Contribution contribution, Guid coordinatorId, string reason)
    {
        var coordinator = await _context.Users.FindAsync(coordinatorId);

        if (coordinator is null)
        {
            throw new Exception("Current authenticated user was not found.");
        }

        // add to contribution activity log.
        await _context.ContributionActivityLogs.AddAsync(new ContributionActivityLog
        {
            ContributionId = contribution.Id,
            ContributionTitle = contribution.Title,
            UserId = coordinator.Id,
            Username = coordinator.UserName,
            FromStatus = contribution.Status,
            ToStatus = ContributionStatus.Reject,
            Description = reason,
        });

        // add to rejection reason table.
        await _context.ContributionRejections.AddAsync(new ContributionRejection
        {
            ContributionId = contribution.Id,
            Reason = reason
        });

        contribution.Status = ContributionStatus.Reject;
        _context.Contributions.Update(contribution);
    }

    public async Task<string> GetRejectionReason(Contribution contribution)
    {
        var reason = await _context.ContributionRejections.FirstOrDefaultAsync(x => x.ContributionId == contribution.Id);
        //var reason = await _context.ContributionActivityLogs.FirstOrDefaultAsync(x => x.ContributionId == contribution.Id && x.ToStatus == ContributionStatus.Reject);

        return reason is not null ? reason.Reason : string.Empty;
    }

    public async Task<PaginationResult<UngradedContributionDto>> GetAllUngradedContributionsPagination(string? keyword, int pageIndex = 1, int pageSize = 10, string? academicYearName = null, string? facultyName = null)
    {
        var query = from c in _context.Contributions
                    where c.DateDeleted == null && c.Status == ContributionStatus.Pending && c.IsCoordinatorCommented == false
                    join u in _context.Users on c.UserId equals u.Id
                    join f in _context.Faculties on c.FacultyId equals f.Id
                    join a in _context.AcademicYears on c.AcademicYearId equals a.Id
                    select new { c, u, f, a };

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.c.Title.Contains(keyword) || x.c.Content.Contains(keyword) || x.c.ShortDescription.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(facultyName))
        {
            query = query.Where(x => x.f.Name == facultyName);
        }

        if (!string.IsNullOrWhiteSpace(academicYearName))
        {
            query = query.Where(x => x.a.Name == academicYearName);
        }

        var rowCount = await query.CountAsync();

        pageIndex = pageIndex - 1 < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        var contributions = await query
            .OrderByDescending(x => x.c.DateCreated)
            .Skip(skipPage)
            .Take(pageSize)
            .ToListAsync();

        // I think this is the only optimized approach here.
        var contributionIds = contributions.Select(x => x.c.Id).ToList();
        var files = await _fileRepository.GetByListContributionIdsAsync(contributionIds);

        var contributionsDto = contributions.Select(x => new UngradedContributionDto
        {
            Id = x.c.Id,
            Title = x.c.Title,
            Slug = x.c.Slug,
            Username = x.u.UserName is not null ? x.u.UserName.ToString() : $"{x.u.FirstName} {x.u.LastName}",
            FacultyName = x.f.Name,
            AcademicYear = x.a.Name,
            SubmissionDate = x.c.DateCreated,
            ShortDescription = x.c.ShortDescription,
            Avatar = x.u.Avatar
        }).ToList();

        return new PaginationResult<UngradedContributionDto>
        {
            CurrentPage = pageIndex,
            RowCount = rowCount,
            PageSize = pageSize,
            Results = contributionsDto
        };
    }
}
