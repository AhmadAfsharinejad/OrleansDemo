using Orleans.Concurrency;

namespace StreamProcessing.Grains.Interfaces;

public interface IOddDetectorGrain : IGrainWithIntegerKey
{
    Task Compute(Immutable<int> index);
}