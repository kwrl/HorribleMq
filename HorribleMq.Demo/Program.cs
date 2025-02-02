using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using HorribleMq;
using HorribleMq.Contract;
using HorribleMq.Demo.Events;
using HorribleMq.FileSystem.AzureBlobStorage;
using HorribleMq.FileSystem.Contract;
using HorribleMq.FileSystem.Local;
using HorribleMq.Serialization.Contract;
using HorribleMq.Serialization.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddSingleton<ISerializer, SystemTextJsonSerializer>();
services.AddSingleton<BlobServiceClient>(_ => new BlobServiceClient("UseDevelopmentStorage=true"));
services.AddSingleton<BlobContainerClient>(sp => sp.GetRequiredService<BlobServiceClient>().GetBlobContainerClient("horrible-mq"));
services.AddSingleton<IFileSystem, AzureBlobStorageFileSystem>();
//services.AddSingleton<IFileSystem>(_ => new LocalFileSystem("/Users/hakonkaurel/horrible-mq"));
services.AddSingleton<FileSystemMessageBus>();
services.AddSingleton<IMessagePublisher>(sp => sp.GetRequiredService<FileSystemMessageBus>());
services.AddSingleton<IMessageSubscriber>(sp => sp.GetRequiredService<FileSystemMessageBus>());
services.AddLogging(options => options.AddConsole());

var serviceProvider = services.BuildServiceProvider();

var containerClient = serviceProvider.GetRequiredService<BlobContainerClient>();
await containerClient.CreateIfNotExistsAsync();
var blobClient = containerClient.GetAppendBlobClient("topics/booking-events.topic");
await blobClient.CreateIfNotExistsAsync();

var publisher = serviceProvider.GetRequiredService<IMessagePublisher>();
var subscriber = serviceProvider.GetRequiredService<IMessageSubscriber>();


var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

var subscriberTask = subscriber.SubscribeAsync<OrderCreated>("booking-events", (order, _) =>
{
    logger.LogInformation("Received order {OrderId}", order.OrderId);
    return Task.CompletedTask;
});

foreach (var _ in Enumerable.Range(0, 1_000))
{
    await publisher.PublishAsync("booking-events", new OrderCreated()
    {
        OrderId = Guid.NewGuid()
    });
    
    await Task.Delay(TimeSpan.FromSeconds(1));
}

await subscriberTask;