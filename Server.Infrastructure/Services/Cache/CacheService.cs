using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Server.Application.Common.Interfaces.Services.Cache;

using StackExchange.Redis;

namespace Server.Infrastructure.Services.Cache;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly CacheSettings _cacheSettings;

    public CacheService(
        IDistributedCache distributedCache,
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<CacheSettings> cacheSettings)
    {
        _distributedCache = distributedCache;
        _connectionMultiplexer = connectionMultiplexer;
        _cacheSettings = cacheSettings.Value;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        string? cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(cachedValue))
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(cachedValue, new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        });
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheSettings.ExpirationMinutes)
        };

        string serializedValue = JsonConvert.SerializeObject(value);
        await _distributedCache.SetStringAsync(key, serializedValue, options, cancellationToken);
    }

    public async Task InvalidateAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }

    // References: https://stackoverflow.com/a/60385140
    public async Task InvalidateWithWildCardAsync(string keyRoot, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(keyRoot))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(keyRoot));

        await foreach (var key in GetKeysAsync($"{keyRoot}*"))
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
    }

    public async IAsyncEnumerable<string> GetKeysAsync(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(pattern));

        foreach (var endpoint in _connectionMultiplexer.GetEndPoints())
        {
            var server = _connectionMultiplexer.GetServer(endpoint);

            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                yield return key.ToString();
            }
        }
    }

    public IEnumerable<RedisFeatures> GetRedisFeatures()
    {
        foreach (var endpoint in _connectionMultiplexer.GetEndPoints())
        {
            var server = _connectionMultiplexer.GetServer(endpoint);
            yield return server.Features;
        }
    }
}
