using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

using Server.Application.Common.Interfaces.Persistence;

using System.Linq.Expressions;

namespace Server.Infrastructure.Services;

public class CacheService<T, TKey> : IRepository<T, TKey> where T : class
{
    protected readonly IRepository<T, TKey> _decorator;
    protected readonly IMemoryCache _memoryCache;
    protected CancellationTokenSource _cacheInvalidationTokenSource;
    protected const int CacheExpirationMinutes = 60;
    private readonly string _cacheKeyPrefix;
    private static readonly HashSet<string> _cacheKeys = new HashSet<string>();

    public CacheService(IRepository<T, TKey> decorator, IMemoryCache memoryCache, string cacheKeyPrefix)
    {
        _decorator = decorator;
        _memoryCache = memoryCache;
        _cacheInvalidationTokenSource = new CancellationTokenSource();
        _cacheKeyPrefix = cacheKeyPrefix;
    }

    protected string GetCacheKey(string key)
    {
        string fullKey = $"{_cacheKeyPrefix}{key}";

        lock (_cacheKeys)
        {
            _cacheKeys.Add(fullKey);
        }

        return fullKey;
    }

    protected MemoryCacheEntryOptions GetCacheOptions()
    {
        return new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes),
            ExpirationTokens = { new CancellationChangeToken(_cacheInvalidationTokenSource.Token) }
        };
    }

    protected void InvalidateCache()
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

    protected Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> factory)
    {
        string cacheKey = GetCacheKey(key);

        return _memoryCache.GetOrCreateAsync(
            cacheKey,
            entry =>
            {
                entry.SetOptions(GetCacheOptions());
                return factory();
            });
    }

    public void Add(T entity)
    {
        _decorator.Add(entity);
        InvalidateCache();
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _decorator.AddRange(entities);
        InvalidateCache();
    }

    public void Remove(T entity)
    {
        _decorator.Remove(entity);
        InvalidateCache();
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _decorator.RemoveRange(entities);
        InvalidateCache();
    }

    public void Update(T entity)
    {
        _decorator.Update(entity);
        InvalidateCache();
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _decorator.UpdateRange(entities);
        InvalidateCache();
    }

    public virtual IEnumerable<T> FindByCondition(Expression<Func<T, bool>> predicate)
    {
        return _decorator.FindByCondition(predicate);
    }

    public virtual Task<IEnumerable<T>> GetAllAsync()
    {
        return GetOrCreateAsync("all", () => _decorator.GetAllAsync());
    }

    public virtual Task<T> GetByIdAsync(TKey id)
    {
        return GetOrCreateAsync($"id-{id}", () => _decorator.GetByIdAsync(id));
    }
}
