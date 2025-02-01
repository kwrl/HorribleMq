
using HorribleMq;
using HorribleMq.Demo.Events;
using HorribleMq.FileSystem.Local;
using HorribleMq.Serialization.Json;

var fileSystem = new LocalFileSystem("/Users/hakonkaurel/horrible-mq");
var serializer = new SystemTextJsonSerializer();
var publisher = new FileSystemMessagePublisher(fileSystem, serializer);

foreach (var _ in Enumerable.Range(0, 10))
{
    await publisher.PublishAsync("booking-events", new OrderCreated()
    {
        OrderId = Guid.NewGuid()
    });
    
    await Task.Delay(TimeSpan.FromMilliseconds(500));
}