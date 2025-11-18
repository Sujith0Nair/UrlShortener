namespace UrlShortener.Contracts;

public interface IUrlQueryService
{
    public Task<string?> GetOriginalUrl(string shortenedUrl);
}