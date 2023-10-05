// using Orleans.Streams;
// using StreamHelloWorld.Domains;
// using StreamHelloWorld.Grains.Interfaces;
//
// namespace StreamHelloWorld.Grains;
//
// [ImplicitStreamSubscription(Consts.PubSubNamespace)]
// public class AsyncObserverConsumerGrain: Grain, IConsumerGrain, IAsyncObserver<int>
// {
//     public override async Task OnActivateAsync(CancellationToken cancellationToken)
//     {
//         var key = this.GetPrimaryKey();
//         
//         await this.GetStreamProvider(Consts.StreamProvider)
//             .GetStream<int>(Consts.PubSubNamespace, key)
//             .SubscribeAsync(this);
//         
//         await base.OnActivateAsync(cancellationToken);
//     }
//
//     public Task OnNextAsync(int item, StreamSequenceToken? token = null)
//     {
//         Console.WriteLine($"AsyncObserver: {item}");
//         return Task.CompletedTask;
//     }
//
//     public Task OnCompletedAsync()
//     {
//         return Task.CompletedTask;
//     }
//
//     public Task OnErrorAsync(Exception ex)
//     {
//         return Task.CompletedTask;
//     }
// }