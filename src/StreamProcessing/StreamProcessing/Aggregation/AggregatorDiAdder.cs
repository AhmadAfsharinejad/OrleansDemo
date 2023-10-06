using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.Aggregation;

internal sealed class AggregatorDiAdder : IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainIntroducer, AggregatorGrainIntroducer>();
    }
}