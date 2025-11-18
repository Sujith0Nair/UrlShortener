namespace UrlShortener.Contracts;

public interface IUrlCreationService
{
    public Task<string?> ShortenUrl(string longUrl);
}