using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.RandomGenerator;

public class RandomGeneratorDiAdder : IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainIntroducer, RandomGeneratorGrainIntroducer>();
    }
}