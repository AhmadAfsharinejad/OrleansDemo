using Orleans.Concurrency;

namespace StreamProcessing.Grains.Interfaces;

public interface IOddDetectorGrain : IGrainWithIntegerKey
{

    [ReadOnly]
    //[OneWay]
    Task Compute(Immutable<int[]> index);
}