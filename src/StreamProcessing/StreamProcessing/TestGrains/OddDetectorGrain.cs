using Orleans.Concurrency;
using StreamProcessing.TestGrains.Interfaces;

namespace StreamProcessing.TestGrains;

[StatelessWorker(5)]
[Reentrant]
public class OddDetectorGrain : Grain, IOddDetectorGrain
{
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("OddDetectorGrain Activated");
        return base.OnActivateAsync(cancellationToken);
    }

    [ReadOnly]
    //[OneWay]
    public async Task Compute(Immutable<int[]> index)
    {
        var last = index.Value.Last();
        //Console.WriteLine(last);

        if (last % 10000 == 0)
            throw new Exception("Bad");

        await Task.CompletedTask;
        //await Task.Delay(TimeSpan.FromMilliseconds(5000)).ConfigureAwait(false);
        return;
    }
}