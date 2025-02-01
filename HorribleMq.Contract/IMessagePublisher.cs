namespace HorribleMq.Contract;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string topicName, T message, CancellationToken stoppingToken = default) where T : class;
}