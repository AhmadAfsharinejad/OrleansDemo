using Orleans.Concurrency;
using StreamProcessing.Grains.Interfaces;

namespace StreamProcessing.Grains;

[StatelessWorker(5)]
//[Reentrant]
public class PassAwayGrain : Grain, IPassAwayGrain
{
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("PassAwayGrain Activated");
        return base.OnActivateAsync(cancellationToken);
    }
    
    //[ReadOnly]
    public async Task Compute(Immutable<int> index)
    {
        var grain = GrainFactory.GetGrain<IOddDetectorGrain>(0);

        await grain.Compute(index).ConfigureAwait(false);
    }
}