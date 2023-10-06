using Orleans.Concurrency;

namespace StreamProcessing.TestGrains.Interfaces;

public interface IPassAwayGrain : IGrainWithIntegerKey
{
    [ReadOnly]
    Task Compute(Immutable<int[]> index);
    Task SayHello();
}