using HorribleMq.FileSystem.Contract;
using Shouldly;

namespace HorribleMq.FileSystem.Local.Test;

public class LocalFileSystemTest
{
    [Fact]
    public async Task ReadAsync_ShouldReadFile()
    {
        var data = "Hello, World!"u8.ToArray();
        var path = "test.txt";
        
        Directory.CreateDirectory(_rootPath);
        await File.WriteAllBytesAsync(Path.Combine(_rootPath, path), data);
        
        var result = await CreateSut().ReadFileAsync(path);
        
        result.ShouldBe(data);
    }
    
    private IFileSystem CreateSut() => new LocalFileSystem(_rootPath);
    
    private readonly string _rootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
}