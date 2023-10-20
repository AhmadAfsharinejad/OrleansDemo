using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Rest.Interfaces;
using StreamProcessing.Rest.Logic;

namespace StreamProcessing.Rest;

internal sealed class RestDiAdder : IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainIntroducer, RestGrainIntroducer>();
        collection.AddSingleton<IRestService, RestService>();
        collection.AddSingleton<IRestRequestCreator, RestRequestCreator>();
        collection.AddSingleton<IRestResponseConverter, RestResponseConverter>();
    }
}