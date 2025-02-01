using HorribleMq.Contract;
using HorribleMq.FileSystem.Contract;

namespace HorribleMq;

public class FileSystemMessagePublisher(IFileSystem fileSystem) : IMessagePublisher
{
    public async Task PublishAsync<T>(
        string topicName, 
        T message, 
        CancellationToken stoppingToken = default
    ) where T : class
    {
        var messageId = Guid.NewGuid();
        await fileSystem.AppendAsync(TopicFileName(topicName), messageId.ToByteArray(), stoppingToken);
    }
    
    private static string TopicFileName(string topicName) => $"{topicName}.topic";
}