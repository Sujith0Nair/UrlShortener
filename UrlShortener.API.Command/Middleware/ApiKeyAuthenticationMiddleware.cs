using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace UrlShortener.API.Command.Middleware;

public class ApiKeyAuthenticationMiddleware(
    RequestDelegate next,
    IConfiguration configuration,
    ILogger<ApiKeyAuthenticationMiddleware> logger)
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    
    public async Task InvokeAsync(HttpContext context)
    {   
        if (!context.Request.Path.Equals("/shorten", StringComparison.OrdinalIgnoreCase) || 
            !context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            logger.LogInformation("Unauthorised request from {ConnectionRemoteIpAddress}", context.Connection.RemoteIpAddress?.ToString());
            await context.Response.WriteAsync("Unauthorized - API Key is not provided");
            return;
        }
        
        var apiKeyConfigPath = $"ApiKeys:{apiKey}";
        var tier = configuration.GetValue<string>(apiKeyConfigPath);

        if (string.IsNullOrEmpty(tier))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            logger.LogInformation("Invalid api key was passed {ApiKey}", $"{apiKey}");
            await context.Response.WriteAsync("Unauthorized - Invalid API Key");
            return;
        }
        
        await next(context);
    }
}