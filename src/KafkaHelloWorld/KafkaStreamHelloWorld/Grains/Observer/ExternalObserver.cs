using KafkaStreamHelloWorld.Grains.Interfaces;
using Orleans.Streams;

namespace KafkaStreamHelloWorld.Grains.Observer;

/// <summary>
/// Class that will log streaming events
/// </summary>
internal sealed class ExternalObserver : IAsyncObserver<object>
{
    private readonly ILogger<IConsumerGrain> _logger;

    public ExternalObserver(ILogger<IConsumerGrain> logger)
    {
        _logger = logger;
    }

    public Task OnCompletedAsync()
    {
        _logger.LogInformation("OnCompletedAsync");
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        _logger.LogInformation("OnErrorAsync: {Exception}", ex);
        return Task.CompletedTask;
    }

    public Task OnNextAsync(object item, StreamSequenceToken? token = null)
    {
        _logger.LogInformation("External Consumer: {item}", item);
        Console.WriteLine($"External Consumer: {item}");
        return Task.CompletedTask;
    }
}