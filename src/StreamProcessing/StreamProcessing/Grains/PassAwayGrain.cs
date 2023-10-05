using Orleans.Concurrency;
using StreamProcessing.Grains.Interfaces;

namespace StreamProcessing.Grains;

[StatelessWorker(5)]
[Reentrant]
public class PassAwayGrain : Grain, IPassAwayGrain
{
    private IOddDetectorGrain grain;
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        grain = GrainFactory.GetGrain<IOddDetectorGrain>(0);

        Console.WriteLine("PassAwayGrain Activated");
        return base.OnActivateAsync(cancellationToken);
    }
    
    [ReadOnly]
    public async Task Compute(Immutable<int> index)
    {
        await grain.Compute(index);
    }
}