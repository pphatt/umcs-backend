using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionPublicBookmarkRepository : IRepository<ContributionPublicBookmark, Guid>
{
    Task<PaginationResult<PublicContributionInListDto>> GetAllBookmarkPagination(string? keyword,
        int pageIndex = 1,
        int pageSize = 10,
        Guid userId = default!,
        string? facultyName = null,
        string? academicYearName = null,
        string? orderBy = null
    );

    Task<bool> AlreadyBookmark(Guid contributionId, Guid userId);

    Task<ContributionPublicBookmark> GetSpecificBookmark(Guid contributionId, Guid userId);
}
