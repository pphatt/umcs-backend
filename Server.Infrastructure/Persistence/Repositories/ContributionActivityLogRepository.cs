using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Extensions;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Common.Enums;
using Server.Domain.Entity.Content;

namespace Server.Infrastructure.Persistence.Repositories;

public class ContributionActivityLogRepository : RepositoryBase<ContributionActivityLog, Guid>, IContributionActivityLogRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFacultyRepository _facultyRepository;
    private readonly IAcademicYearRepository _academicYearRepository;

    public ContributionActivityLogRepository(AppDbContext context, IMapper mapper, IFacultyRepository facultyRepository, IAcademicYearRepository academicYearRepository) : base(context)
    {
        _context = context;
        _mapper = mapper;
        _facultyRepository = facultyRepository;
        _academicYearRepository = academicYearRepository;
    }

    public async Task<PaginationResult<ContributionActivityLogDto>> GetAllContributionActivityLogsPagination(
        int pageIndex = 1,
        int pageSize = 10,
        string? facultyName = null,
        string? academicYearName = null,
        string? orderBy = null)
    {
        var query = _context.ContributionActivityLogs
            .Where(x => x.DateDeleted == null)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(facultyName))
        {
            var faculty = await _facultyRepository.GetFacultyByNameAsync(facultyName);

            // single query execution + sub-query join more performance than using list async.
            query = query.Where(x => _context.Contributions
                .Where(c => c.FacultyId == faculty.Id)
                .Select(c => c.Id)
                .Contains(x.ContributionId));
        }

        if (!string.IsNullOrWhiteSpace(academicYearName))
        {
            var academicYear = await _academicYearRepository.GetAcademicYearByNameAsync(academicYearName);

            // same here.
            query = query.Where(x => _context.Contributions
                .Where(x => x.AcademicYearId == academicYear.Id)
                .Select(x => x.Id)
                .Contains(x.ContributionId));
        }

        var isAscending = !string.IsNullOrWhiteSpace(orderBy) &&
                          Enum.TryParse<OrderByEnum>(orderBy, true, out var enumOrderBy) &&
                          enumOrderBy == OrderByEnum.Ascending;

        if (isAscending)
        {
            query = query.OrderBy(x => x.DateCreated);
        }
        else
        {
            query = query.OrderByDescending(x => x.DateCreated);
        }

        var rowCount = await query.CountAsync();

        pageIndex = pageIndex - 1 < 0 ? 1 : pageIndex;

        var skipPage = (pageIndex - 1) * pageSize;

        var activityList = await query
            .Skip(skipPage)
            .Take(pageSize)
            .ToListAsync();

        var result = new List<ContributionActivityLogDto>();

        foreach (var activity in activityList)
        {
            var a = _mapper.Map<ContributionActivityLogDto>(activity);

            a.FromStatus = activity.FromStatus.ToStringValue();
            a.ToStatus = activity.ToStatus.ToStringValue();

            result.Add(a);
        }

        return new PaginationResult<ContributionActivityLogDto>
        {
            CurrentPage = pageIndex,
            RowCount = rowCount,
            PageSize = pageSize,
            Results = result
        };
    }

    public async Task<List<ContributionActivityLogDto>> GetContributionActivityLogsByContribution(Contribution contribution)
    {
        // I think the logs of a single contribution is not that many so no need to paginate this.
        // contribution activity logs.
        var cals = await _context.ContributionActivityLogs.Where(x => x.ContributionId == contribution.Id).ToListAsync();

        var result = new List<ContributionActivityLogDto>();

        foreach (var log in cals)
        {
            var dto = _mapper.Map<ContributionActivityLogDto>(log);

            dto.FromStatus = log.FromStatus.ToStringValue();
            dto.ToStatus = log.ToStatus.ToStringValue();

            result.Add(dto);
        }

        return result;
    }
}
