﻿using Orleans.Streams;

namespace StreamHelloWorld.Grains;


class Observer : IAsyncObserver<int>
{
    public Task OnCompletedAsync()
    {
        Console.WriteLine("OnCompletedAsync");
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        Console.WriteLine("OnErrorAsync: {Exception}", ex);
        return Task.CompletedTask;
    }

    public Task OnNextAsync(int item, StreamSequenceToken? token = null)
    {
        Console.WriteLine(item);
        return Task.CompletedTask;
    }
}