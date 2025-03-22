namespace Server.Application.Common.Interfaces.Services.Cache;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    Task InvalidateAsync(string key, CancellationToken cancellationToken = default);
    Task InvalidateWithWildCardAsync(string keyRoot, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> GetKeysAsync(string pattern);
}
