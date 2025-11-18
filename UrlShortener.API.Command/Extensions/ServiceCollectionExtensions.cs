using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.API.Command.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services,
        IConfiguration _)
    {
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.AddPolicy("api-key-policy", httpContext =>
            {
                httpContext.Request.Headers.TryGetValue("X-Api-Key", out var apiKey);
                var tier = httpContext.RequestServices.GetRequiredService<IConfiguration>()[$"ApiKeys:{apiKey}"];
                var permitLimit = tier switch
                {
                    "pro-tier" => 1000,
                    "free-tier" => 1,
                    _ => 10
                };
                var partitionKey = !string.IsNullOrEmpty(apiKey) ? apiKey.ToString() : httpContext.Connection.RemoteIpAddress!.ToString();
                return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromDays(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });
            
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            rateLimiterOptions.OnRejected = (context, token) =>
            {
                context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
                return ValueTask.CompletedTask;
            };
        });
        return services;
    }
}