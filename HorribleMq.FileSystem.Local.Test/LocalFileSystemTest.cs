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
        
        var result = await CreateSut().ReadAsync(path);
        
        result.ShouldBe(data);
    }
    
    [Fact]
    public async Task ReadAsync_ShouldReadFileWithSkipAndTake()
    {
        var data = "Hello, World!"u8.ToArray();
        var path = "test.txt";
        
        Directory.CreateDirectory(_rootPath);
        await File.WriteAllBytesAsync(Path.Combine(_rootPath, path), data);
        
        var result = await CreateSut().ReadAsync(path, 0, 5);
       
        result.Length.ShouldBe(5);
    }
    
    private IFileSystem CreateSut() => new LocalFileSystem(_rootPath);
    
    private readonly string _rootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
}