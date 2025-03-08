using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionActivityLogRepository : IRepository<ContributionActivityLog, Guid>
{
    Task<PaginationResult<ContributionActivityLogDto>> GetAllContributionActivityLogsPagination(
        int pageIndex = 1,
        int pageSize = 10,
        string? facultyName = null,
        string? academicYearName = null,
        string? orderBy = null);

    Task<List<ContributionActivityLogDto>> GetContributionActivityLogsByContribution(Contribution contribution);
}
