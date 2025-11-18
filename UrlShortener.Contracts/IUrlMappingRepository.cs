using UrlShortener.Domain;

namespace UrlShortener.Contracts;

public interface IUrlMappingRepository
{
    public Task<UrlMapping?> CreateUrlMappingIntoDb(string originalUrl);
    public Task<bool> UpdateShortUrlAsync(long id, string shortUrl);
    public Task<string?> GetOriginalUrl(string shortenedUrl);
    public Task<bool> IncrementUrlClickCount(string shortenedUrl);
}