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

        var t1 = Run();
        //var t2 = Run();
        //var t3 = Run();

        await Task.WhenAll(t1);

        Console.WriteLine($"Finished {DateTime.Now}");
    }

    private async Task Run()
    {
        var grain = _grainFactory.GetGrain<IPassAwayGrain>(0);

        var batchCount = 10;
        var items = new int[batchCount];
        int index = 0;

        for (int i = 1; i < 100000000; i++)
        {
            items[index++]  = i;

            if (index == batchCount)
            {

                try
                {
                    await grain.Compute(items.AsImmutable());
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
                items = new int[batchCount];
                index = 0;
            }

            // if (i % 1000 == 0)
            // {
            //     await Task.WhenAll(tasks);
            //     tasks.Clear();
            // }
        }
    }
}