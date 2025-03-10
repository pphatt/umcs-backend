using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;

namespace Server.Application.Common.Interfaces.Persistence.Repositories;

public interface IAcademicYearRepository : IRepository<AcademicYear, Guid>
{
    Task<AcademicYear> GetAcademicYearByNameAsync(string academicYearName);

    Task<bool> HasContributionsAsync(Guid academicYearId);

    Task<PaginationResult<AcademicYearDto>> GetAllAcademicYearsPagination(string? keyword, int pageIndex = 1, int pageSize = 10);

    Task<bool> CanSubmitAsync(DateTime date);

    Task<AcademicYear?> GetAcademicYearByDateAsync(DateTime date);

    Task<AcademicYear?> GetAcademicYearByYearAsync(DateTime date);
}
