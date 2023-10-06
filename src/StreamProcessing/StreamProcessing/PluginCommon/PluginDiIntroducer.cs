using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.PluginCommon;

public class PluginDiIntroducer : IDependencyIntroducer
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainFactory, PluginGrainFactory>();
    }
}