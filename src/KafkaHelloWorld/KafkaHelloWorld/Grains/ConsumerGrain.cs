﻿using KafkaHelloWorld.Domains;
using KafkaHelloWorld.Grains.Interfaces;
using Orleans.Providers;
using Orleans.Streams;

namespace KafkaHelloWorld.Grains;

[StorageProvider(ProviderName = "Default")]
public class ConsumerGrain: Grain, IConsumerGrain
{
    public Task SayHello()
    {
        return Task.CompletedTask;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var kafkaProvider = this.GetStreamProvider(Consts.StreamProvider);
        var testStream = kafkaProvider.GetStream<object>(Consts.FirstTopic,"id1");

        // To resume stream in case of stream deactivation
        var subscriptionHandles = await testStream.GetAllSubscriptionHandles();
        if (subscriptionHandles.Count > 0)
        {
            foreach (var subscriptionHandle in subscriptionHandles)
            {
                await subscriptionHandle.ResumeAsync(OnNextTestMessage);
            }
        }

        await testStream.SubscribeAsync(OnNextTestMessage);
    }

    private Task OnNextTestMessage(object message, StreamSequenceToken sequenceToken)
    {
        Console.BackgroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(message?.ToString());
        return Task.CompletedTask;
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        return base.OnDeactivateAsync(reason, cancellationToken);
    }
}