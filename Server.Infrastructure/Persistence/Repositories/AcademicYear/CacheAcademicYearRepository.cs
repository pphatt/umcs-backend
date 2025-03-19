using System.Linq.Expressions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Common.Interfaces.Persistence.Repositories;
using Server.Application.Wrapper.Pagination;

namespace Server.Infrastructure.Persistence.Repositories.AcademicYear;

using AcademicYear = Server.Domain.Entity.Content.AcademicYear;

public class CacheAcademicYearRepository : IAcademicYearRepository
{
    private readonly AcademicYearRepository _decorator;
    private readonly IMemoryCache _memoryCache;
    private CancellationTokenSource _cacheInvalidationTokenSource;
    private const int CacheExpirationMinutes = 60;
    private const string CacheKeyPrefix = "academic-year:";
    private static readonly HashSet<string> _cacheKeys = new HashSet<string>();

    public CacheAcademicYearRepository(AcademicYearRepository decorator, IMemoryCache memoryCache)
    {
        _decorator = decorator;
        _memoryCache = memoryCache;
        _cacheInvalidationTokenSource = new CancellationTokenSource();
    }

    private void InvalidateCache()
    {
        if (_cacheInvalidationTokenSource != null)
        {
            _cacheInvalidationTokenSource.Cancel();
            _cacheInvalidationTokenSource.Dispose();
            _cacheInvalidationTokenSource = new CancellationTokenSource();
        }

        lock (_cacheKeys)
        {
            foreach (var key in _cacheKeys)
            {
                _memoryCache.Remove(key);
            }

            _cacheKeys.Clear();
        }
    }

    private string GetCacheKey(string key)
    {
        string cacheKey = $"{CacheKeyPrefix}{key}";

        lock (_cacheKeys)
        {
            _cacheKeys.Add(cacheKey);
        }

        return cacheKey;
    }

    private MemoryCacheEntryOptions GetCacheOptions()
    {
        return new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes),
            ExpirationTokens = { new CancellationChangeToken(_cacheInvalidationTokenSource.Token) }
        };
    }

    public void Add(AcademicYear entity)
    {
        _decorator.Add(entity);
        InvalidateCache();
    }

    public void AddRange(IEnumerable<AcademicYear> entities)
    {
        _decorator.AddRange(entities);
        InvalidateCache();
    }

    public void Remove(AcademicYear entity)
    {
        _decorator.Remove(entity);
        InvalidateCache();
    }

    public void RemoveRange(IEnumerable<AcademicYear> entities)
    {
        _decorator.RemoveRange(entities);
        InvalidateCache();
    }

    public void Update(AcademicYear entity)
    {
        _decorator.Update(entity);
        InvalidateCache();
    }

    public void UpdateRange(IEnumerable<AcademicYear> entities)
    {
        _decorator.UpdateRange(entities);
        InvalidateCache();
    }

    public IEnumerable<AcademicYear> FindByCondition(Expression<Func<AcademicYear, bool>> predicate)
    {
        return _decorator.FindByCondition(predicate);
    }

    public Task<bool> CanSubmitAsync(DateTime date)
    {
        string key = GetCacheKey($"can-submit-{date:yyyy-MM-dd}");

        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetOptions(GetCacheOptions());

                return _decorator.CanSubmitAsync(date);
            });
    }

    public Task<AcademicYear?> GetAcademicYearByDateAsync(DateTime date)
    {
        string key = GetCacheKey($"by-date-{date:yyyy-MM-dd}");

        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetOptions(GetCacheOptions());

                return _decorator.GetAcademicYearByDateAsync(date);
            });
    }

    public Task<AcademicYear> GetAcademicYearByNameAsync(string academicYearName)
    {
        string key = GetCacheKey($"by-name-{academicYearName}");

        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetOptions(GetCacheOptions());

                return _decorator.GetAcademicYearByNameAsync(academicYearName);
            });
    }

    public Task<AcademicYear?> GetAcademicYearByYearAsync(DateTime date)
    {
        string key = GetCacheKey($"by-year-{date.Year}");

        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetOptions(GetCacheOptions());
                return _decorator.GetAcademicYearByYearAsync(date);
            });
    }

    public Task<PaginationResult<AcademicYearDto>> GetAllAcademicYearsPagination(string? keyword, int pageIndex = 1, int pageSize = 10)
    {
        string key = GetCacheKey($"pagination-{keyword ?? "all"}-{pageIndex}-{pageSize}");

        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetOptions(GetCacheOptions());

                return _decorator.GetAllAcademicYearsPagination(keyword, pageIndex, pageSize);
            });
    }

    public Task<IEnumerable<AcademicYear>> GetAllAsync()
    {
        string key = GetCacheKey("all");

        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetOptions(GetCacheOptions());

                return _decorator.GetAllAsync();
            });
    }

    public Task<AcademicYear> GetByIdAsync(Guid id)
    {
        string key = GetCacheKey($"id-{id}");

        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetOptions(GetCacheOptions());

                return _decorator.GetByIdAsync(id);
            });
    }

    public Task<bool> HasContributionsAsync(Guid academicYearId)
    {
        string key = GetCacheKey($"has-contributions-{academicYearId}");

        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetOptions(GetCacheOptions());

                return _decorator.HasContributionsAsync(academicYearId);
            });
    }
}
