using UrlShortener.Contracts;

namespace UrlShortener.Application;

public class UrlCreationService(ICacheService cacheService, IUrlMappingRepository urlMappingRepository, IUrlShorteningService urlShorteningService) : IUrlCreationService
{
    public async Task<string?> ShortenUrl(string longUrl)
    {
        var mapping = await urlMappingRepository.CreateUrlMappingIntoDb(longUrl);
        if (mapping == null)
        {
            return null;
        }
        
        var shortUrl = urlShorteningService.Encode(mapping.Id);
        var isUpdateSuccessful = await urlMappingRepository.UpdateShortUrlAsync(mapping.Id, shortUrl);
        if (!isUpdateSuccessful)
        {
            return null;
        }
        
        await cacheService.SetAsync(shortUrl, longUrl);
        return shortUrl;
    }
}