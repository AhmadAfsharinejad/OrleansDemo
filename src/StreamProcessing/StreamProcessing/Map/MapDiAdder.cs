using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.Map.Interfaces;
using StreamProcessing.Map.Logic;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.Map;

internal sealed class MapDiAdder : IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainIntroducer, MapGrainIntroducer>();
        collection.AddSingleton<ICompiler, Compiler>();
    }
}