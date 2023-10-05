﻿using Orleans.Streams;

namespace StreamHelloWorld.Grains.Observers;


class Observer : IAsyncObserver<int>
{
    public Task OnCompletedAsync()
    {
       // Console.WriteLine("OnCompletedAsync");
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        //Console.WriteLine("OnErrorAsync: {Exception}", ex);
        return Task.CompletedTask;
    }

    public Task OnNextAsync(int item, StreamSequenceToken? token = null)
    {
        if(item % 10000 == 0)
            Console.WriteLine(item);
        
        //Console.WriteLine($"Observer: {item}");
        return Task.CompletedTask;
    }
}