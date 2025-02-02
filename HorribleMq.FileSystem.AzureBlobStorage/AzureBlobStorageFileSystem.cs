using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using HorribleMq.FileSystem.Contract;

namespace HorribleMq.FileSystem.AzureBlobStorage;

public class AzureBlobStorageFileSystem(BlobContainerClient containerClient) : IFileSystem
{
    public async Task<byte[]> ReadFileAsync(string path, CancellationToken stoppingToken = default)
    {
        var blobClient = containerClient.GetBlobClient(path);
        var response = await blobClient.DownloadAsync(stoppingToken);
        using var ms = new MemoryStream();
        await response.Value.Content.CopyToAsync(ms, stoppingToken);
        return ms.ToArray();
    }

    public async Task<byte[]> ReadFileAsync(string path, long skip, long take, CancellationToken stoppingToken = default)
    {
        var blobClient = containerClient.GetBlobClient(path);
        var response = await blobClient.DownloadAsync(new HttpRange(skip, take), cancellationToken: stoppingToken);
        using var ms = new MemoryStream();
        await response.Value.Content.CopyToAsync(ms, stoppingToken);
        return ms.ToArray();
    }

    public async Task<long> GetFileSizeAsync(string path, CancellationToken stoppingToken = default)
    {
        var blobClient = containerClient.GetBlobClient(path);
        var properties = await blobClient.GetPropertiesAsync(cancellationToken: stoppingToken);
        return properties.Value.ContentLength;
    }

    public Task CreateFileAsync(string path, byte[] data, CancellationToken stoppingToken = default)
    {
        var blobClient = containerClient.GetBlobClient(path);
        return blobClient.UploadAsync(new BinaryData(data), cancellationToken: stoppingToken);
    }

    public Task AppendToFileAsync(string path, byte[] data, CancellationToken stoppingToken = default)
    {
        var blobClient = containerClient.GetAppendBlobClient(path);
        using var ms = new MemoryStream(data);
        return blobClient.AppendBlockAsync(ms, cancellationToken: stoppingToken);
    }
}