namespace StreamProcessing.TestGrains.Interfaces;

public interface IIntRandomGeneratorGrain : IGrainWithIntegerKey
{
    Task Compute();
}