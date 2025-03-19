using Microsoft.Extensions.Caching.Memory;

using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;
using Server.Infrastructure.Services;

namespace Server.Infrastructure.Persistence.Repositories.AcademicYear;

using AcademicYear = Server.Domain.Entity.Content.AcademicYear;

public class CacheAcademicYearRepository : CacheService<AcademicYear, Guid>, IAcademicYearRepository
{
    private readonly AcademicYearRepository _academicYearRepository;
    private const string CacheKeyPrefix = "academic-year:";

    public CacheAcademicYearRepository(AcademicYearRepository decorator, IMemoryCache memoryCache)
        : base(decorator, memoryCache, CacheKeyPrefix)
    {
        _academicYearRepository = decorator;
    }

    public override Task<IEnumerable<AcademicYear>> GetAllAsync()
    {
        return GetOrCreateAsync("all", () => _academicYearRepository.GetAllAsync());
    }

    public override Task<AcademicYear> GetByIdAsync(Guid id)
    {
        return GetOrCreateAsync($"id-{id}", () => _academicYearRepository.GetByIdAsync(id));
    }

    public Task<bool> CanSubmitAsync(DateTime date)
    {
        return GetOrCreateAsync($"can-submit-{date:yyyy-MM-dd}", () => _academicYearRepository.CanSubmitAsync(date));
    }

    public Task<AcademicYear?> GetAcademicYearByDateAsync(DateTime date)
    {
        return GetOrCreateAsync($"by-date-{date:yyyy-MM-dd}", () => _academicYearRepository.GetAcademicYearByDateAsync(date));
    }

    public Task<AcademicYear> GetAcademicYearByNameAsync(string academicYearName)
    {
        return GetOrCreateAsync($"by-name-{academicYearName}", () => _academicYearRepository.GetAcademicYearByNameAsync(academicYearName));
    }

    public Task<AcademicYear?> GetAcademicYearByYearAsync(DateTime date)
    {
        return GetOrCreateAsync($"by-year-{date.Year}", () => _academicYearRepository.GetAcademicYearByYearAsync(date));
    }

    public Task<PaginationResult<AcademicYearDto>> GetAllAcademicYearsPagination(string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        return GetOrCreateAsync($"pagination-{keyword ?? "all"}-{pageIndex}-{pageSize}",
            () => _academicYearRepository.GetAllAcademicYearsPagination(keyword, pageIndex, pageSize));
    }

    public Task<bool> HasContributionsAsync(Guid academicYearId)
    {
        return GetOrCreateAsync($"has-contributions-{academicYearId}", () => _academicYearRepository.HasContributionsAsync(academicYearId));
    }
}
