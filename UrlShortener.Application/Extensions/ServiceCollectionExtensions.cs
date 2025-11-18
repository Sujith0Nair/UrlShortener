using UrlShortener.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandServices(this IServiceCollection services)
    {
        services.AddScoped<IUrlCreationService, UrlCreationService>();
        services.AddSingleton<IUrlShorteningService, UrlShorteningService>();
        return services;
    }
    
    public static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IUrlQueryService, UrlQueryService>();
        return services;
    }
}