using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionPublicReadLaterRepository : IRepository<ContributionPublicReadLater, Guid>
{
    Task<PaginationResult<PublicContributionInListDto>> GetAllReadLaterPublicContributionPagination(Guid userId, string? keyword, int pageIndex = 1, int pageSize = 10, string? facultyName = null, string? academicYearName = null, string? orderBy = null);

    Task<bool> AlreadySave(Guid contributionId, Guid userId);

    Task<ContributionPublicReadLater> GetSpecificSave(Guid contributionId, Guid userId);
}
