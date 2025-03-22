using System.Linq.Expressions;

using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Common.Interfaces.Services.Cache;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Content;
using Server.Infrastructure.Persistence.Repositories;

namespace Server.Infrastructure.Caching;

public class CacheFacultyRepository : IFacultyRepository
{
    private readonly FacultyRepository _decorator;
    private readonly ICacheService _cacheService;
    private const string FACULTY = "faculty";
    private const string FACULTY_PAGINATION = "faculty-pagination";

    public CacheFacultyRepository(FacultyRepository decorator, ICacheService cacheService)
    {
        _decorator = decorator;
        _cacheService = cacheService;
    }

    private async Task UpdateCacheAsync(Faculty entity)
    {
        string keyById = $"{FACULTY}:id={entity.Id}";
        string keyByName = $"{FACULTY}:name={entity.Name}";
        string keyByPagination = FACULTY_PAGINATION;

        // Invalidate existing cache entries
        await _cacheService.InvalidateAsync(keyById);
        await _cacheService.InvalidateAsync(keyByName);
        await _cacheService.InvalidateWithWildCardAsync(keyByPagination);

        // Set new cache entries
        await _cacheService.SetAsync(keyById, entity);
        await _cacheService.SetAsync(keyByName, entity);
    }

    private async Task InvalidateCacheAsync(Faculty entity)
    {
        string keyById = $"{FACULTY}:id={entity.Id}";
        string keyByName = $"{FACULTY}:name={entity.Name}";
        string keyByPagination = FACULTY_PAGINATION;

        await _cacheService.InvalidateAsync(keyById);
        await _cacheService.InvalidateAsync(keyByName);
        await _cacheService.InvalidateWithWildCardAsync(keyByPagination);
    }

    public void Add(Faculty entity)
    {
        _decorator.Add(entity);
        UpdateCacheAsync(entity).GetAwaiter().GetResult();
    }

    public void AddRange(IEnumerable<Faculty> entities)
    {
        _decorator.AddRange(entities);

        foreach (var entity in entities)
        {
            UpdateCacheAsync(entity).GetAwaiter().GetResult();
        }
    }

    public void Remove(Faculty entity)
    {
        _decorator.Remove(entity);
        InvalidateCacheAsync(entity).GetAwaiter().GetResult();
    }

    public void RemoveRange(IEnumerable<Faculty> entities)
    {
        _decorator.RemoveRange(entities);

        foreach (var entity in entities)
        {
            InvalidateCacheAsync(entity).GetAwaiter().GetResult();
        }
    }

    public void Update(Faculty entity)
    {
        _decorator.Update(entity);
        UpdateCacheAsync(entity).GetAwaiter().GetResult();
    }

    public void UpdateRange(IEnumerable<Faculty> entities)
    {
        _decorator.UpdateRange(entities);

        foreach (var entity in entities)
        {
            UpdateCacheAsync(entity).GetAwaiter().GetResult();
        }
    }

    public IEnumerable<Faculty> FindByCondition(Expression<Func<Faculty, bool>> predicate)
    {
        return _decorator.FindByCondition(predicate);
    }

    public async Task<IEnumerable<Faculty>> GetAllAsync()
    {
        string key = $"{FACULTY}:all";

        var cached = await _cacheService.GetAsync<IEnumerable<Faculty>>(key);

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

    public async Task<Faculty> GetByIdAsync(Guid id)
    {
        string key = $"{FACULTY}:id={id}";

        var cached = await _cacheService.GetAsync<Faculty>(key);

        if (cached is not null)
        {
            return cached;
        }

        var faculty = await _decorator.GetByIdAsync(id);

        if (faculty is not null)
        {
            await _cacheService.SetAsync(key, faculty);
        }

        return faculty;
    }

    public async Task<int> Count()
    {
        string key = $"{FACULTY}:count";

        var cached = await _cacheService.GetAsync<int>(key);

        if (cached != 0)
        {
            return cached;
        }

        var count = await _decorator.Count();

        if (count != 0)
        {
            await _cacheService.SetAsync(key, count);
        }

        return count;
    }

    public async Task<PaginationResult<FacultyDto>> GetAllFacultiesPagination(
        string? keyword,
        int pageIndex = 1,
        int pageSize = 10)
    {
        string key = $"{FACULTY_PAGINATION}:limit={pageSize}:offset={pageIndex}:keyword={keyword ?? "none"}";

        var cached = await _cacheService.GetAsync<PaginationResult<FacultyDto>>(key);

        if (cached is not null)
        {
            return cached;
        }

        var result = await _decorator.GetAllFacultiesPagination(keyword, pageIndex, pageSize);

        if (result.Results.Count > 0)
        {
            await _cacheService.SetAsync(key, result);
        }

        return result;
    }

    public async Task<Faculty> GetFacultyByNameAsync(string name)
    {
        string key = $"{FACULTY}:name={name}";

        var cached = await _cacheService.GetAsync<Faculty>(key);

        if (cached is not null)
        {
            return cached;
        }

        var faculty = await _decorator.GetFacultyByNameAsync(name);

        if (faculty is not null)
        {
            await _cacheService.SetAsync(key, faculty);
        }

        return faculty;
    }
}
