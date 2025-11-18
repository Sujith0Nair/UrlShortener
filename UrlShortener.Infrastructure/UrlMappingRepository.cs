using UrlShortener.Domain;
using UrlShortener.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Infrastructure;

public class UrlMappingRepository(ApplicationDbContext dbContext, ILogger<UrlMappingRepository> logger) : IUrlMappingRepository
{
    public async Task<UrlMapping?> CreateUrlMappingIntoDb(string originalUrl)
    {
        var entryData = new UrlMapping
        {
            OriginalUrl = originalUrl
        };
        await dbContext.UrlMappings.AddAsync(entryData);
        var changes = await dbContext.SaveChangesAsync();
        return changes > 0 ? entryData : null;
    }

    public async Task<bool> UpdateShortUrlAsync(long id, string shortUrl)
    {
        var urlMapping = await dbContext.UrlMappings.FirstOrDefaultAsync(x => x.Id == id);
        if (urlMapping == null)
        {
            return false;
        }
        urlMapping.ShortenedUrl = shortUrl;
        var changes = await dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<string?> GetOriginalUrl(string shortenedUrl)
    {
        var item = await dbContext.UrlMappings.FirstOrDefaultAsync(x => x.ShortenedUrl == shortenedUrl);
        return item?.OriginalUrl;
    }

    public async Task<bool> IncrementUrlClickCount(string shortenedUrl)
    {
        var urlMapping = await dbContext.UrlMappings.FirstOrDefaultAsync(mapping => mapping.ShortenedUrl == shortenedUrl);
        if (urlMapping == null)
        {
            logger.LogWarning("Failed to increment click count for {ShortUrl}. Link might not exist.", shortenedUrl);
            return false;
        }
        
        urlMapping.ClickCount++;
        dbContext.UrlMappings.Update(urlMapping);
        
        var changes = await dbContext.SaveChangesAsync();
        return changes > 0;
    }
}