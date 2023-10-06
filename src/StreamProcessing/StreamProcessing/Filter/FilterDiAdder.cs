using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.Filter;

internal sealed class FilterDiAdder : IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainIntroducer, FilterGrainIntroducer>();
    }
}