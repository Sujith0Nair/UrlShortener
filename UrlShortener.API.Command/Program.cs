using Scalar.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using UrlShortener.API.Command.Middleware;
using UrlShortener.API.Command.Extensions;
using UrlShortener.Application.Extensions;
using UrlShortener.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.API.Command;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        
        builder.Services.AddCommandServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddRateLimitingPolicies(builder.Configuration);
        
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.MapScalarApiReference();
            app.MapOpenApi();
        }
        
        app.UseHttpsRedirection();
        app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
        app.UseRateLimiter();
        app.UseAuthorization();
        app.MapControllers();
        
        app.Run();
    }
}