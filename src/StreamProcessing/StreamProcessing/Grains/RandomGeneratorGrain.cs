using Orleans.Concurrency;
using StreamProcessing.Grains.Interfaces;

namespace StreamProcessing.Grains;

public class RandomGeneratorGrain : Grain, IRandomGeneratorGrain
{
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("RandomGeneratorGrain Activated");
        return base.OnActivateAsync(cancellationToken);
    }
    
    public async Task Compute()
    {
        var grain = GrainFactory.GetGrain<IPassAwayGrain>(0);

        for (int i = 0; i < 10000; i++)
        {
            await grain.Compute(i.AsImmutable());
        }
    }
}