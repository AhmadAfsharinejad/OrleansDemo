using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.HttpResponse.Interfaces;
using StreamProcessing.HttpResponse.Logic;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.HttpResponse;

internal sealed class HttpResponseDiAdder : IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainIntroducer, HttpResponseGrainIntroducer>();
        collection.AddSingleton<IHttpResponseService, HttpResponseService>();
    }
}