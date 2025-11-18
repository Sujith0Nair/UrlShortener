using UrlShortener.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.API.Query.Controllers;

[ApiController]
public class UrlShortenerController(IUrlQueryService urlQueryService) : ControllerBase
{   
    [HttpGet("{shortUrl}")]
    public async Task<ActionResult> GetOriginalUrl(string shortUrl)
    {
        if (string.IsNullOrEmpty(shortUrl) || string.IsNullOrWhiteSpace(shortUrl))
        {
            return BadRequest("Passed url is null or empty. Cannot get original url.");
        }
        
        var originalUrl = await urlQueryService.GetOriginalUrl(shortUrl);
        if (originalUrl == null)
        {
            return BadRequest("No entries for such shortened url is found!");
        }
        
        return Redirect(originalUrl);
    }
}