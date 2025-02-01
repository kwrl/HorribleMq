namespace HorribleMq.FileSystem.Contract;

public interface IFileSystem 
{
    Task<byte[]> ReadFileAsync(
        string path, 
        CancellationToken stoppingToken = default
    );

    Task CreateFileAsync(
        string path,
        byte[] data,
        CancellationToken stoppingToken = default
    );
    
    Task AppendToFileAsync(
        string path,
        byte[] data,
        CancellationToken stoppingToken = default
    );
}