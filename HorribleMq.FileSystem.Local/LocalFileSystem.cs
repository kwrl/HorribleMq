using HorribleMq.FileSystem.Contract;

namespace HorribleMq.FileSystem.Local;

public class LocalFileSystem(string rootPath) : IFileSystem
{
    public async Task<byte[]> ReadFileAsync(string path, CancellationToken stoppingToken = default)
    {
        var fullPath = Path.Combine(rootPath, path);
        await using var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var buffer = new byte[fs.Length];
        
        var bytesRead = await fs.ReadAsync(buffer, stoppingToken);
        
        if (bytesRead < fs.Length)
        {
            Array.Resize(ref buffer, bytesRead);
        }
        
        return buffer;
    }

    public async Task CreateFileAsync(string path, byte[] data, CancellationToken stoppingToken = default)
    {
        var fullPath = Path.Combine(rootPath, path);

        await using var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await fs.WriteAsync(data.AsMemory(), stoppingToken);
    }

    public async Task AppendToFileAsync(string path, byte[] data, CancellationToken stoppingToken = default)
    {
        var fullPath = Path.Combine(rootPath, path);

        await using var fs = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.None);
        await fs.WriteAsync(data.AsMemory(), stoppingToken);
    }
}