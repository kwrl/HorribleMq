using HorribleMq.FileSystem.Contract;

namespace HorribleMq.FileSystem.Local;

public class LocalFileSystem(string rootPath) : IFileSystem
{
    public async Task<byte[]> ReadAsync(string path, int skip, int take, CancellationToken stoppingToken = default)
    {
        var buffer = new byte[take];
        var fullPath = Path.Combine(rootPath, path);
        await using var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var bytesRead = await fs.ReadAsync(buffer.AsMemory(skip, take), stoppingToken);
       
        if (bytesRead < take)
        {
            Array.Resize(ref buffer, bytesRead);
        }
        
        return buffer;
    }

    public async Task<byte[]> ReadAsync(string path, CancellationToken stoppingToken = default)
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

    public async Task WriteAsync(string path, byte[] data, int skip = 0, CancellationToken stoppingToken = default)
    {
        var fullPath = Path.Combine(rootPath, path);

        await using var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Write, FileShare.None);
        await fs.WriteAsync(data.AsMemory(skip, data.Length), stoppingToken);
    }

    public async Task AppendAsync(string path, byte[] data, CancellationToken stoppingToken = default)
    {
        var fullPath = Path.Combine(rootPath, path);

        await using var fs = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.None);
        await fs.WriteAsync(data.AsMemory(), stoppingToken);
    }
}