using StreamHelloWorld.Grains.Interfaces;
using Orleans.Runtime;
using Orleans.Streams;
using StreamHelloWorld.Domains;

namespace StreamHelloWorld.Grains;

public class ProducerGrain : Grain, IProducerGrain
{
    private IAsyncStream<int>? _stream;
    private IDisposable? _timer;
    private int _counter = 0;

    public Task StartProducing(Guid key)
    {
        var streamProvider = this.GetStreamProvider(Consts.StreamProvider);
        var streamId = StreamId.Create(Consts.PubSubNamespace, key);
        _stream = streamProvider.GetStream<int>(streamId);

        var period = TimeSpan.FromSeconds(1);
        _timer = RegisterTimer(TimerTick, null, period, period);

        return Task.CompletedTask;
    }

    private async Task TimerTick(object _)
    {
        var value = _counter++;
        if (_stream is not null)
        {
            await _stream.OnNextAsync(value);
        }
    }

    public Task StopProducing()
    {
        if (_timer is not null)
        {
            _timer.Dispose();
            _timer = null;
        }

        if (_stream is not null)
        {
            _stream = null;
        }

        return Task.CompletedTask;
    }
}