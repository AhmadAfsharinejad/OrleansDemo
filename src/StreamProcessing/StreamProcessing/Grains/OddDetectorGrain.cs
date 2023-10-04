using Orleans.Concurrency;
using StreamProcessing.Grains.Interfaces;

namespace StreamProcessing.Grains;

[StatelessWorker(5)]
//[Reentrant]
public class OddDetectorGrain : Grain, IOddDetectorGrain
{
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("OddDetectorGrain Activated");
        return base.OnActivateAsync(cancellationToken);
    }

    //[ReadOnly]
    public async Task Compute(Immutable<int> index)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(5000)).ConfigureAwait(false);
        return;
    }
}