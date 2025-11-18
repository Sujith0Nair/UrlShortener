using UrlShortener.Contracts;
using UrlShortener.Contracts.Events;

namespace UrlShortener.Application;

public class UrlQueryService(ICacheService cacheService, IUrlMappingRepository urlMappingRepository, IMessagePublisher messagePublisher) : IUrlQueryService
{
    private const string RoutingKey = "link.visited";
    
    public async Task<string?> GetOriginalUrl(string shortUrl)
    {
        var originalUrl = await cacheService.GetAsync(shortUrl);
        if (originalUrl != null)
        {
            await RecordLinkVisitAsync(shortUrl);
            return originalUrl;
        }

        originalUrl = await urlMappingRepository.GetOriginalUrl(shortUrl);
        if (originalUrl == null) return null;
        
        await cacheService.SetAsync(shortUrl, originalUrl);
        await RecordLinkVisitAsync(shortUrl);
        return originalUrl;
    }
    
    private async Task RecordLinkVisitAsync(string shortUrl)
    {
        var linkVisitedEvent = new LinkVisitedEvent
        {
            ShortUrl = shortUrl,
            VisitedAtUtc = DateTime.UtcNow
        };
        await messagePublisher.PublishAsync(linkVisitedEvent, RoutingKey);
    }
}