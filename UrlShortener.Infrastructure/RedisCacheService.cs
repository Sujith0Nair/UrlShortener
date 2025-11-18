using StackExchange.Redis;
using UrlShortener.Contracts;

namespace UrlShortener.Infrastructure;

public class RedisCacheService(IConnectionMultiplexer connectionMultiplexer) : ICacheService
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
    
    public async Task<string?> GetAsync(string key)
    {
        var value = await _database.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    public async Task<bool> SetAsync(string key, string value, TimeSpan? absoluteExpiration = null)
    {
        return await _database.StringSetAsync(key, value, absoluteExpiration);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }
}