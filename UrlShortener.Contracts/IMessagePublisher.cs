namespace UrlShortener.Contracts;

public interface IMessagePublisher
{
    public Task PublishAsync<T>(T message, string routingKey) where T : class;
}