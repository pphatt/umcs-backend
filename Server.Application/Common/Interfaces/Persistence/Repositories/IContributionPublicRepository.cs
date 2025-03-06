using Server.Application.Common.Dtos.Content.Like;
using Server.Application.Common.Dtos.Content.PublicContribution;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionPublicRepository : IRepository<ContributionPublic, Guid>
{
    Task<PaginationResult<PublicContributionInListDto>> GetAllPublicContributionsPagination(
        string? keyword,
        int pageIndex = 1,
        int pageSize = 10,
        string? academicYearName = null,
        string? facultyName = null,
        bool? allowedGuest = null,
        string? sortBy = null,
        string? orderBy = null);

    Task<List<ContributorDto>> GetTopContributors(
        string? keyword,
        int pageIndex = 1,
        int pageSize = 10,
        string? facultyName = null,
        string? orderBy = null
    );

    Task<PublicContributionDto> GetPublicContributionBySlug(string slug);

    Task<PaginationResult<UserLikeInListDto>> GetAllUsersLikedContributionPagination(Guid contributionId, int pageIndex = 1, int pageSize = 10);
}
