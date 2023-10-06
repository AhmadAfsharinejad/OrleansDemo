using StreamProcessing.TestGrains.Interfaces;

namespace StreamProcessing.TestGrains;

public class IntRandomGeneratorGrain : Grain, IIntRandomGeneratorGrain
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
            //await grain.Compute(i.AsImmutable());
        }
    }
}