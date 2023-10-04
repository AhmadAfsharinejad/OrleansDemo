namespace StreamProcessing.Grains.Interfaces;

public interface IRandomGeneratorGrain : IGrainWithIntegerKey
{
    Task Compute();
}