using RabbitMQ.Client;
using StackExchange.Redis;
using UrlShortener.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextService(configuration);
        services.AddCachingService(configuration);
        services.AddMessagingService(configuration);
        return services;
    }

    private static void AddCachingService(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration.GetConnectionString("RedisConnection") ??
                              throw new InvalidOperationException("Redis connection string not found. Please setup to continue.");
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
        services.AddSingleton<ICacheService, RedisCacheService>();
    }

    private static void AddMessagingService(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqConnection = configuration.GetConnectionString("RabbitMqConnection");
        var rabbitMqFactory = new ConnectionFactory
        {
            Uri = new Uri(rabbitMqConnection ?? throw new InvalidOperationException("RabbitMQ connection string not found. Please setup to continue.")),
        };
        services.AddSingleton<IConnection>(_ => rabbitMqFactory.CreateConnectionAsync().GetAwaiter().GetResult());
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
        services.AddHostedService<LinkVisitedEventConsumer>();
    }

    private static void AddDbContextService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddScoped<IUrlMappingRepository, UrlMappingRepository>();
    }
}