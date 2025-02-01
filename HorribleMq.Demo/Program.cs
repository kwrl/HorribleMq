
using HorribleMq;
using HorribleMq.FileSystem.Local;

var fileSystem = new LocalFileSystem();
var publisher = new FileSystemMessagePublisher(fileSystem);

Console.WriteLine("Hello, World!");