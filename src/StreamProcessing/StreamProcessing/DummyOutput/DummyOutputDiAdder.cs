using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.DummyOutput;

internal sealed class DummyOutputDiAdder: IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainIntroducer, DummyOutputGrainIntroducer>();
    }
}