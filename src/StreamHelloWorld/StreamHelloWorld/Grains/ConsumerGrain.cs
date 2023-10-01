using Orleans.Streams;
using Orleans.Streams.Core;
using StreamHelloWorld.Domains;
using StreamHelloWorld.Grains.Interfaces;

namespace StreamHelloWorld.Grains;

[ImplicitStreamSubscription(Consts.PubSubNamespace)]
public class ConsumerGrain: Grain, IConsumerGrain, IStreamSubscriptionObserver
{
    private readonly Observer _observer;
    private IStreamProvider? StreamProvider;

    public ConsumerGrain()
    {
        _observer = new Observer();
    }
    
    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<int>();
        await handle.ResumeAsync(_observer);
    }
    
    // public override async Task OnActivateAsync(CancellationToken cancellationToken)
    // {
    //     // StreamProvider cannot be obtained outside the Orleans lifecycle methods
    //     StreamProvider = this.GetStreamProvider(Consts.StreamProvider);
    //
    //     var _sub = await StreamProvider
    //         .GetStream<object>(this.GetPrimaryKey().ToString(), Consts.PubSubNamespace)
    //         .SubscribeAsync(HandleAsync);
    //
    //     await base.OnActivateAsync(cancellationToken);
    // }


    public Task<int> HandleAsync(object evt, StreamSequenceToken token)
    {
        Console.WriteLine(evt);
        return Task.FromResult(1);
    }
}
