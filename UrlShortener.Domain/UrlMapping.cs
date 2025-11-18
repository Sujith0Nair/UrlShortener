namespace UrlShortener.Domain;

public class UrlMapping
{
    public long Id { get; init; }
    public string OriginalUrl { get; init; } = "";
    public string ShortenedUrl { get; set; } = "";
    public int ClickCount { get; set; }
}