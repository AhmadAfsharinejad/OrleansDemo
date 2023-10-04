using Orleans.Concurrency;

namespace StreamProcessing.Grains.Interfaces;

public interface IPassAwayGrain : IGrainWithIntegerKey
{
    Task Compute(Immutable<int> index);
}