using HorribleMq.Contract;
using HorribleMq.FileSystem.Contract;
using HorribleMq.Serialization.Contract;

namespace HorribleMq;

public class FileSystemMessagePublisher(
    IFileSystem fileSystem,
    ISerializer serializer
) : IMessagePublisher
{
    public async Task PublishAsync<T>(
        string topicName, 
        T message, 
        CancellationToken stoppingToken = default
    ) where T : class
    {
        var messageId = Guid.NewGuid();
        var payload = serializer.Serialize(message);
        await fileSystem.CreateFileAsync(DataFilePath(messageId), payload, stoppingToken);
        await fileSystem.AppendToFileAsync(TopicFilePath(topicName), messageId.ToByteArray(), stoppingToken);
    }
    
    private static string TopicFilePath(string topicName) => Path.Combine("topics", $"{topicName}.topic");
    private static string DataFilePath(Guid messageId) => Path.Combine("data", $"{messageId}.data");
}