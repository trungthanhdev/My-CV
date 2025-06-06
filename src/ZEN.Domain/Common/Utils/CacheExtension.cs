namespace Microsoft.Extensions.Caching.Hybrid;

public static class HybridCacheExtensions
{
    private static readonly HybridCacheEntryOptions _options = new() { Flags = HybridCacheEntryFlags.DisableUnderlyingData };

    public async static Task<bool> ExistsAsync(this HybridCache cache, string key)
    {
        (var exists, _) = await TryGetValueAsync<object>(cache, key);
        return exists;
    }

    public async static Task<(bool, T?)> TryGetValueAsync<T>(this HybridCache cache, string key)
    {
        var result = await cache.GetOrCreateAsync(
            key,
            async _ => await NoOpAsync<T>(),
            _options);

        return (result != null, result);
    }

    private static async ValueTask<T> NoOpAsync<T>() => await Task.FromResult<T>(default!);
}