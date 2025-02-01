namespace HorribleMq.Serialization.Contract;

public interface ISerializer
{
    byte[] Serialize<T>(T obj);
    T Deserialize<T>(byte[] data);
}