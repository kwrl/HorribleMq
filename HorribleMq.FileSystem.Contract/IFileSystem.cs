namespace HorribleMq.FileSystem.Contract;

public interface IFileSystem 
{
    Task<byte[]> ReadAsync(
        string path, 
        int skip, 
        int take,
        CancellationToken stoppingToken = default
    );
    
    Task<byte[]> ReadAsync(
        string path, 
        CancellationToken stoppingToken = default
    );

    Task WriteAsync(
        string path,
        byte[] data,
        int skip = 0,
        CancellationToken stoppingToken = default
    );
    
    Task AppendAsync(
        string path,
        byte[] data,
        CancellationToken stoppingToken = default
    );
}