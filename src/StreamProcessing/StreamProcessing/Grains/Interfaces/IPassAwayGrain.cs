using Orleans.Concurrency;

namespace StreamProcessing.Grains.Interfaces;

public interface IPassAwayGrain : IGrainWithIntegerKey
{
    [ReadOnly]
    //[OneWay]
    Task Compute(Immutable<int[]> index);
}