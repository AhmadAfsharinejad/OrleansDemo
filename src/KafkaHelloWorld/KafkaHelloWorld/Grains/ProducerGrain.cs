using KafkaHelloWorld.Domains;
using KafkaHelloWorld.Grains.Interfaces;
using Orleans.Streams;

namespace KafkaHelloWorld.Grains;

public class ProducerGrain: Grain, IProducerGrain
{
    private IAsyncStream<object>? _stream;

    public async Task StartProducing()
    {
        var streamProvider = this.GetStreamProvider(Consts.StreamProvider);
        _stream = streamProvider.GetStream<object>(Consts.FirstTopic, "id1");
        for (int i = 0; i < 1000000000; i++)
        {
            await _stream.OnNextAsync(12345);
        }
    }
}