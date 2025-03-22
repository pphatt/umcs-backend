using System.Linq.Expressions;

using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Common.Interfaces.Services.Cache;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;
using Server.Infrastructure.Persistence.Repositories;

namespace Server.Infrastructure.Caching;

public class CacheAcademicYearRepository : IAcademicYearRepository
{
    private readonly AcademicYearRepository _decorator;
    private readonly ICacheService _cacheService;
    private const string ACADEMIC_YEAR = "academic-year";
    private const string ACADEMIC_YEAR_PAGINATION = "academic-year-pagination";

    public CacheAcademicYearRepository(AcademicYearRepository decorator, ICacheService cacheService)
    {
        _decorator = decorator;
        _cacheService = cacheService;
    }

    private async Task UpdateCacheAsync(AcademicYear entity)
    {
        string keyById = $"{ACADEMIC_YEAR}:id={entity.Id}";
        string keyByName = $"{ACADEMIC_YEAR}:name={entity.Name}";
        string keyByPagination = ACADEMIC_YEAR_PAGINATION;

        // Invalidate existing cache entries
        await _cacheService.InvalidateAsync(keyById);
        await _cacheService.InvalidateAsync(keyByName);
        await _cacheService.InvalidateWithWildCardAsync(keyByPagination);

        // Set new cache entries
        await _cacheService.SetAsync(keyById, entity);
        await _cacheService.SetAsync(keyByName, entity);
    }

    private async Task InvalidateCacheAsync(AcademicYear entity)
    {
        string keyById = $"{ACADEMIC_YEAR}:id={entity.Id}";
        string keyByName = $"{ACADEMIC_YEAR}:name={entity.Name}";
        string keyByPagination = ACADEMIC_YEAR_PAGINATION;

        await _cacheService.InvalidateAsync(keyById);
        await _cacheService.InvalidateAsync(keyByName);
        await _cacheService.InvalidateWithWildCardAsync(keyByPagination);
    }

    public void Add(AcademicYear entity)
    {
        _decorator.Add(entity);
        UpdateCacheAsync(entity).GetAwaiter().GetResult();
    }

    public void AddRange(IEnumerable<AcademicYear> entities)
    {
        _decorator.AddRange(entities);

        foreach (var entity in entities)
        {
            UpdateCacheAsync(entity).GetAwaiter().GetResult();
        }
    }

    public void Remove(AcademicYear entity)
    {
        _decorator.Remove(entity);
        InvalidateCacheAsync(entity).GetAwaiter().GetResult();
    }

    public void RemoveRange(IEnumerable<AcademicYear> entities)
    {
        _decorator.RemoveRange(entities);

        foreach (var entity in entities)
        {
            InvalidateCacheAsync(entity).GetAwaiter().GetResult();
        }
    }

    public void Update(AcademicYear entity)
    {
        _decorator.Update(entity);
        UpdateCacheAsync(entity).GetAwaiter().GetResult();
    }

    public void UpdateRange(IEnumerable<AcademicYear> entities)
    {
        _decorator.UpdateRange(entities);

        foreach (var entity in entities)
        {
            UpdateCacheAsync(entity).GetAwaiter().GetResult();
        }
    }

    public IEnumerable<AcademicYear> FindByCondition(Expression<Func<AcademicYear, bool>> predicate)
    {
        return _decorator.FindByCondition(predicate);
    }

    public Task<bool> CanSubmitAsync(DateTime date)
    {
        return _decorator.CanSubmitAsync(date);
    }

    public async Task<IEnumerable<AcademicYear>> GetAllAsync()
    {
        string key = $"{ACADEMIC_YEAR}:all";

        var cached = await _cacheService.GetAsync<IEnumerable<AcademicYear>>(key);

        if (cached is not null)
        {
            return cached;
        }

        var result = await _decorator.GetAllAsync();

        if (result is not null)
        {
            await _cacheService.SetAsync(key, result);
        }

        return result;
    }

    public async Task<AcademicYear> GetByIdAsync(Guid id)
    {
        string key = $"{ACADEMIC_YEAR}:id={id}";

        var cached = await _cacheService.GetAsync<AcademicYear>(key);

        if (cached is not null)
        {
            return cached;
        }

        var academicYear = await _decorator.GetByIdAsync(id);

        if (academicYear is not null)
        {
            await _cacheService.SetAsync(key, academicYear);
        }

        return academicYear;
    }

    public Task<AcademicYear?> GetAcademicYearByDateAsync(DateTime date)
    {
        return _decorator.GetAcademicYearByDateAsync(date);
    }

    public async Task<AcademicYear> GetAcademicYearByNameAsync(string academicYearName)
    {
        string key = $"{ACADEMIC_YEAR}:name={academicYearName}";

        var cached = await _cacheService.GetAsync<AcademicYear>(key);

        if (cached is not null)
        {
            return cached;
        }

        var academicYear = await _decorator.GetAcademicYearByNameAsync(academicYearName);

        if (academicYear is not null)
        {
            await _cacheService.SetAsync(key, academicYear);
        }

        return academicYear;
    }

    public Task<AcademicYear?> GetAcademicYearByYearAsync(DateTime date)
    {
        return _decorator.GetAcademicYearByYearAsync(date);
    }

    public async Task<PaginationResult<AcademicYearDto>> GetAllAcademicYearsPagination(
        string? keyword,
        int pageIndex = 1,
        int pageSize = 10)
    {
        string key = $"{ACADEMIC_YEAR_PAGINATION}:limit={pageSize}:offset={pageIndex}:keyword={keyword ?? "none"}";

        var cached = await _cacheService.GetAsync<PaginationResult<AcademicYearDto>>(key);

        if (cached is not null)
        {
            return cached;
        }

        var result = await _decorator.GetAllAcademicYearsPagination(keyword, pageIndex, pageSize);

        if (result.Results.Count > 0)
        {
            await _cacheService.SetAsync(key, result);
        }

        return result;
    }

    public Task<bool> HasContributionsAsync(Guid academicYearId)
    {
        return _decorator.HasContributionsAsync(academicYearId);
    }
}
