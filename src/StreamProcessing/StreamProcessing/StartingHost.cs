using System.Runtime.InteropServices.JavaScript;
using Microsoft.Extensions.Hosting;
using Orleans.Concurrency;
using StreamProcessing.Grains.Interfaces;

namespace StreamProcessing;

internal sealed class StartingHost : BackgroundService
{
    private readonly IGrainFactory _grainFactory;

    public StartingHost(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"Start {DateTime.Now}");
        
        // var generator = _grainFactory.GetGrain<IRandomGeneratorGrain>(0);
        // generator.Compute();

        await Run();
        
        Console.WriteLine($"Finished {DateTime.Now}");
    }

    private async Task Run()
    {
        var grain = _grainFactory.GetGrain<IPassAwayGrain>(0);
        var tasks = new List<Task>();
        for (int i = 0; i < 100000; i++)
        {
            var task = grain.Compute(i.AsImmutable());
            tasks.Add(task);

            // if (i % 1000 == 0)
            // {
            //     await Task.WhenAll(tasks);
            //     tasks.Clear();
            // }
        }

        await Task.WhenAll(tasks);
    }
}