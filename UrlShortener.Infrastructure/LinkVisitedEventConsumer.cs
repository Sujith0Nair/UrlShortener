using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using RabbitMQ.Client.Events;
using UrlShortener.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using UrlShortener.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.Infrastructure;

public class LinkVisitedEventConsumer(
    ILogger<LinkVisitedEventConsumer> logger,
    IConnection connection,
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    private const string ExchangeName = "url_shortener_exchange";
    private const string RoutingKey = "link.visited";
    private const string QueueName = "link.visited.queue";
    
    private IChannel? _channel;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("LinkVisitedEventConsumer started.");

        try
        {
            _channel ??= await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);
            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null, 
                cancellationToken: cancellationToken);
            await _channel.QueueBindAsync(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: RoutingKey,
                cancellationToken: cancellationToken);
            
            logger.LogInformation($"Queue {QueueName} declared and bound to exchange {ExchangeName} with routing key {RoutingKey}");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to declare and bind queue to exchange");
            throw;
        }
        
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        if (_channel == null)
        {
            logger.LogError("RabbitMq channel was not initialised, cannot consume messages!");
            return;
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            logger.LogInformation("Received message: {Message}", message);

            try
            {
                var linkVisitedEvent = JsonSerializer.Deserialize<LinkVisitedEvent>(message);
                if (linkVisitedEvent == null)
                {
                    logger.LogError("Failed to deserialize message : {Message}", message);
                    return;
                }
                logger.LogInformation("Processing LinkVisitedEvent for ShortUrl: {ShortUrl} at {VisitedAtUtc}", linkVisitedEvent.ShortUrl, linkVisitedEvent.VisitedAtUtc);
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<IUrlMappingRepository>();
                    var success = await repo.IncrementUrlClickCount(linkVisitedEvent.ShortUrl);
                    if (success)
                    {
                        logger.LogInformation("Successfully incremented click count for {ShortUrl}", linkVisitedEvent.ShortUrl);
                    }
                    else
                    {
                        logger.LogWarning("Failed to increment click count for {ShortUrl}. Link might not exist.", linkVisitedEvent.ShortUrl);
                    }
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                logger.LogInformation("Message acknowledged {Message}", message);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error processing {Message}", message);
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(QueueName, false, consumer, cancellationToken: stoppingToken);
        logger.LogInformation("Started consuming messages from queue {QueueName}", QueueName);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("LinkVisitedEventConsumer is stopping.");

        if (_channel is { IsOpen: true })
        {
            await _channel.CloseAsync(cancellationToken: cancellationToken);
            await _channel.DisposeAsync();
        }

        if (connection is { IsOpen: true })
        {
            await connection.CloseAsync(cancellationToken: cancellationToken);
            await connection.DisposeAsync();
        }
        
        logger.LogInformation("RabbitMQ connection and channel closed.");
        await base.StopAsync(cancellationToken);
    }
}