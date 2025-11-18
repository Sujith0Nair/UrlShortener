namespace UrlShortener.Contracts.Events;

public class LinkVisitedEvent
{
    public string ShortUrl { get; set; } = string.Empty;
    public DateTime VisitedAtUtc { get; set; } = DateTime.UtcNow;
}