using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using UrlShortener.Contracts;

namespace UrlShortener.Infrastructure;

public class RabbitMqPublisher(IConnection connection) : IMessagePublisher, IDisposable
{
    private const string ExchangeName = "url_shortener_exchange";
    private IChannel? _channel;

    public async Task PublishAsync<T>(T message, string routingKey) where T : class
    {
        _channel ??= await connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic, durable: true);
        var messageBody = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(messageBody);
        await _channel.BasicPublishAsync(ExchangeName, routingKey, false, body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        GC.SuppressFinalize(this);
    }
}