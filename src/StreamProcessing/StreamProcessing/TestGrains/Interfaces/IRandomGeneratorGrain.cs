namespace StreamProcessing.TestGrains.Interfaces;

public interface IRandomGeneratorGrain : IGrainWithIntegerKey
{
    Task Compute();
}