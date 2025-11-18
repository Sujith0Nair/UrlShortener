namespace UrlShortener.Contracts;

public interface ICacheService
{
    public Task<string?> GetAsync(string key);
    public Task<bool> SetAsync(string key, string value, TimeSpan? absoluteExpiration = null);
    public Task<bool> RemoveAsync(string key);
}