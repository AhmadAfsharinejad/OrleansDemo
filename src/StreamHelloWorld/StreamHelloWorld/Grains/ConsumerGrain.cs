using Orleans.Streams.Core;
using StreamHelloWorld.Grains.Interfaces;

namespace StreamHelloWorld.Grains;

[ImplicitStreamSubscription("StreamNameSpace")]
public class ConsumerGrain: Grain, IConsumerGrain, IStreamSubscriptionObserver
{
    private readonly Observer _observer;

    public ConsumerGrain()
    {
        _observer = new Observer();
    }
    
    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<int>();
        await handle.ResumeAsync(_observer);
    }
}
