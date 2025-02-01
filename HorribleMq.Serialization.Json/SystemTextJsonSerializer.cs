using System.Text.Json;
using HorribleMq.Serialization.Contract;

namespace HorribleMq.Serialization.Json;

public class SystemTextJsonSerializer : ISerializer
{
    public byte[] Serialize<T>(T obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj);
    }

    public T Deserialize<T>(byte[] data)
    {
        return JsonSerializer.Deserialize<T>(data) ?? throw new InvalidOperationException();
    }
}