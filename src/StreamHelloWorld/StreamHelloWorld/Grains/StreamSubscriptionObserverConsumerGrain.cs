using Orleans.Streams.Core;
using StreamHelloWorld.Domains;
using StreamHelloWorld.Grains.Interfaces;
using StreamHelloWorld.Grains.Observers;

namespace StreamHelloWorld.Grains;

[ImplicitStreamSubscription(Consts.PubSubNamespace)]
public class StreamSubscriptionObserverConsumerGrain: Grain, IConsumerGrain, IStreamSubscriptionObserver
{
    private readonly Observer _observer;

    public StreamSubscriptionObserverConsumerGrain()
    {
        _observer = new Observer();
    }
    
    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<int>();
        await handle.ResumeAsync(_observer);
    }
}
