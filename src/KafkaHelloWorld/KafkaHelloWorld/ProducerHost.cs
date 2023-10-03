using KafkaHelloWorld.Grains.Interfaces;

namespace KafkaHelloWorld;

public sealed class ProducerHost : BackgroundService
{
    private readonly IGrainFactory _grainFactory;
    private IProducerGrain? _grain;

    public ProducerHost(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //TODO 
        // await Task.CompletedTask;
        // return;

        _grain = _grainFactory.GetGrain<IProducerGrain>("id1");
        await _grain.StartProducing();
    }
}