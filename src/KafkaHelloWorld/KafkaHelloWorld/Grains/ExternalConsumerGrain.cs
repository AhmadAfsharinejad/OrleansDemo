using KafkaHelloWorld.Domains;
using KafkaHelloWorld.Grains.Interfaces;
using KafkaHelloWorld.Grains.Observer;
using Orleans.Streams.Core;

namespace KafkaHelloWorld.Grains;

[ImplicitStreamSubscription(Consts.ExternalTopic)]
internal sealed class ExternalConsumerGrain: Grain, IConsumerGrain, IStreamSubscriptionObserver
{
    private readonly ILogger<IConsumerGrain> _logger;
    private readonly ExternalObserver _observer;

    public ExternalConsumerGrain(ILogger<IConsumerGrain> logger)
    {
        _logger = logger;
        _observer = new ExternalObserver(_logger);
    }

    // Called when a subscription is added
    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        // Plug our LoggerObserver to the stream
        var handle = handleFactory.Create<object>();
        await handle.ResumeAsync(_observer);
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OnActivateAsync");
        return Task.CompletedTask;
    }

    public Task Active()
    {
        return Task.CompletedTask;
    }
}
