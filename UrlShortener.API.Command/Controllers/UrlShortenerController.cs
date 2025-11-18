using UrlShortener.Contracts;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Contracts.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace UrlShortener.API.Command.Controllers;

[ApiController]
public class UrlShortenerController(IUrlCreationService urlCreationService) : ControllerBase
{   
    [HttpPost("shorten")]
    [EnableRateLimiting("api-key-policy")]
    public async Task<ActionResult> ShortenUrl([FromBody] ShortenUrlRequestData requestData)
    {
        var longUrl = requestData.LongUrl;
        
        if (string.IsNullOrEmpty(longUrl) || string.IsNullOrWhiteSpace(longUrl))
        {
            return BadRequest("Passed url is null or empty. Cannot shorten url.");
        }

        var shortUrl = await urlCreationService.ShortenUrl(longUrl);
        if (shortUrl == null)
        {
            return BadRequest("Could not shorten url!");
        }

        return Content(shortUrl, "text/plain");
    }
}