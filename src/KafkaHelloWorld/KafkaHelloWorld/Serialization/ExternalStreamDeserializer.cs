using System.Text;
using Orleans.Streams.Utils;
using Orleans.Streams.Utils.Serialization;

namespace KafkaHelloWorld.Serialization;

public class ExternalStreamDeserializer :  IExternalStreamDeserializer
{
    public object Deserialize(QueueProperties queueProps, Type type, byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    public void Dispose() { }
};
