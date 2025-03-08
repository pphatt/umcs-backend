using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IContributionRepository : IRepository<Contribution, Guid>
{
    Task<bool> IsSlugAlreadyExisted(string slug, Guid? contributionId = null);

    Task<PaginationResult<ContributionInListDto>> GetAllContributionsPagination(
        string? keyword,
        int pageIndex = 1,
        int pageSize = 10,
        Guid? userId = null,
        string? facultyName = null,
        string? academicYearName = null,
        bool? allowedGuest = null,
        string? status = null,
        string? orderBy = null);

    Task<ContributionDto> GetContributionBySlugAndFaculty(string slug, Guid facultyId);

    Task<ContributionDto> GetPersonalContributionBySlug(string slug, Guid userId);

    Task<ContributionDto> GetContributionBySlug(string slug);

    Task SendToApproved(Guid contributionId, Guid studentId);

    Task ApproveContribution(Contribution contribution, Guid coordinatorId);

    Task RejectContribution(Contribution contribution, Guid coordinatorId, string reason);

    Task<PaginationResult<UngradedContributionDto>> GetAllUngradedContributionsPagination(
        string? keyword,
        int pageIndex = 1,
        int pageSize = 10,
        string? academicYearName = null,
        string? facultyName = null,
        string? orderBy = null);
}
