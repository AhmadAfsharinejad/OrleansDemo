using Orleans.Concurrency;

namespace StreamProcessing.TestGrains.Interfaces;

public interface IOddDetectorGrain : IGrainWithIntegerKey
{

    [ReadOnly]
    //[OneWay]
    Task Compute(Immutable<int[]> index);
}