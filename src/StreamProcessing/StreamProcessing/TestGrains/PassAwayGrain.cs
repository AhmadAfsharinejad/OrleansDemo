using Orleans.Concurrency;
using StreamProcessing.TestGrains.Interfaces;

namespace StreamProcessing.TestGrains;

[StatelessWorker(4)]
//[Reentrant]
public class PassAwayGrain : Grain, IPassAwayGrain
{
    private IOddDetectorGrain grain;
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        grain = GrainFactory.GetGrain<IOddDetectorGrain>(0);

        Console.WriteLine($"PassAwayGrain Activated  {this.GetPrimaryKeyLong()}, {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }
    
    [ReadOnly]
    //[OneWay]
    public async Task Compute(Immutable<int[]> index)
    {
        await grain.Compute(index);
    }

    public Task SayHello()
    {
        Console.WriteLine($"hello from {this.GetPrimaryKeyLong()}, {this.GetGrainId()}");
        return Task.CompletedTask;
    }
}