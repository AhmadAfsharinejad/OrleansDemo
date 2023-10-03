using StreamHelloWorld.Grains.Interfaces;

namespace StreamHelloWorld;

public class StartingHost: IHostedService, IDisposable
{
    private readonly IGrainFactory _grainFactory;
    private IProducerGrain? _grain;

    public StartingHost(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        _grain = _grainFactory.GetGrain<IProducerGrain>(id);
        await _grain.StartProducing(Guid.NewGuid());
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _grain?.StopProducing();
    }

    public void Dispose()
    {
        //nothing to do
    }
}