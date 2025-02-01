namespace HorribleMq.Contract;

public interface IMessageSubscriber
{
    Task SubscribeAsync<T>(string topicName, Func<T, CancellationToken, Task> handler, CancellationToken stoppingToken = default) where T : class;
}