// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;
using StreamHelloWorld.Grains.Interfaces;

Console.WriteLine("Hello, World!");


var serviceCollection = new ServiceCollection();
serviceCollection.AddOrleans(siloBuilder =>
{
    siloBuilder
        .AddMemoryStreams<DefaultMemoryMessageBodySerializer>("StreamProvider")
        .AddMemoryGrainStorage("PubSubStore");
});
// serviceCollection.AddOrleansClient( clientBuilder =>
// {
//     clientBuilder
//         .AddMemoryStreams<DefaultMemoryMessageBodySerializer>("StreamProvider");
// });


var provider = serviceCollection.BuildServiceProvider();


var client = provider.GetRequiredService<IGrainFactory>();

// Use the connected client to ask a grain to start producing events
var key = Guid.NewGuid();
var producer = client.GetGrain<IProducerGrain>("my-producer");
await producer.StartProducing("StreamNameSpace", key);

// var streamId = StreamId.Create("StreamNameSpace", key);
// var stream = client
//     .GetStreamProvider("StreamNameSpace")
//     .GetStream<int>(streamId);
// await stream.SubscribeAsync(OnNextAsync);
//
// Console.ReadLine();


static Task OnNextAsync(int item, StreamSequenceToken? token = null)
{
    Console.WriteLine("OnNextAsync: item: {0}, token = {1}", item, token);
    return Task.CompletedTask;
}