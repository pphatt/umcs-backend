using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface ILikeRepository : IRepository<Like, Guid>
{
    Task<PaginationResult<PublicContributionInListDto>> GetAllUserLikePublicContributionsPagination(string? keyword = null,
        int pageIndex = 1,
        int pageSize = 10,
        Guid userId = default!,
        string? facultyName = null,
        string? academicYearName = null,
        string? orderBy = null);

    Task<bool> AlreadyLike(Guid contributionId, Guid userId);

    Task<Like> GetSpecificLike(Guid contributionId, Guid userId);
}
