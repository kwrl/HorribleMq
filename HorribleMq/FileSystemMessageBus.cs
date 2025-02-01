using HorribleMq.Contract;
using HorribleMq.FileSystem.Contract;
using HorribleMq.Serialization.Contract;
using Microsoft.Extensions.Logging;

namespace HorribleMq;

public class FileSystemMessageBus(
    IFileSystem fileSystem,
    ISerializer serializer,
    ILogger<FileSystemMessageBus> logger
) : IMessagePublisher, IMessageSubscriber
{
    public async Task PublishAsync<T>(
        string topicName, 
        T message, 
        CancellationToken stoppingToken = default
    ) where T : class
    {
        var messageId = Guid.NewGuid();
        logger.LogInformation("Publishing message {MessageId} to topic {TopicName}", messageId, topicName);
        
        var payload = serializer.Serialize(message);
        await fileSystem.CreateFileAsync(DataFilePath(messageId), payload, stoppingToken);
        await fileSystem.AppendToFileAsync(TopicFilePath(topicName), messageId.ToByteArray(), stoppingToken);
        
        logger.LogInformation("Published message {MessageId} to topic {TopicName}", messageId, topicName);
    }
    
    public async Task SubscribeAsync<T>(string topicName, Func<T, CancellationToken, Task> handler, CancellationToken stoppingToken = default) where T : class
    {
        var index = 0;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            long offset = index*16;
           
            var fileSize = await fileSystem.GetFileSizeAsync(TopicFilePath(topicName), stoppingToken);
            
            if (offset >= fileSize)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                continue;
            }
            
            var bytes = await fileSystem.ReadFileAsync(TopicFilePath(topicName), offset, 16, stoppingToken);
            var messageId = new Guid(bytes);
            var payload = await fileSystem.ReadFileAsync(DataFilePath(messageId), stoppingToken);
            var message = serializer.Deserialize<T>(payload);
            
            await handler.Invoke(message, stoppingToken);
            
            index++;
        }
    }
    
    private static string TopicFilePath(string topicName) => Path.Combine("topics", $"{topicName}.topic");
    private static string DataFilePath(Guid messageId) => Path.Combine("data", $"{messageId}.data");
}