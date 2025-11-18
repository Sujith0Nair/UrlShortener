namespace UrlShortener.Contracts;

public interface IUrlShorteningService
{
    public string Encode(long id);
}