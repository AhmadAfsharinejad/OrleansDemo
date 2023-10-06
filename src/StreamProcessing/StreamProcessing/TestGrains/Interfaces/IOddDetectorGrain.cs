using Orleans.Concurrency;

namespace StreamProcessing.TestGrains.Interfaces;

public interface IOddDetectorGrain : IGrainWithIntegerKey
{

    [ReadOnly]
    Task Compute(Immutable<int[]> index);
}