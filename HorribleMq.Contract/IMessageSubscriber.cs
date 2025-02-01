namespace HorribleMq.Contract;

public interface IMessageSubscriber
{
    Task SubscribeAsync<T>(string topicName, Func<T, Task> handler, CancellationToken stoppingToken = default) where T : class;
}