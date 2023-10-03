using KafkaHelloWorld.Grains.Interfaces;

namespace KafkaHelloWorld;

public sealed class ConsumerHost : BackgroundService
{
    private readonly IGrainFactory _grainFactory;
    private IConsumerGrain? _grain;

    public ConsumerHost(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //TODO 
        // await Task.CompletedTask;
        // return;
        
        _grain = _grainFactory.GetGrain<IConsumerGrain>("id1");
        await _grain.SayHello();
    }
}