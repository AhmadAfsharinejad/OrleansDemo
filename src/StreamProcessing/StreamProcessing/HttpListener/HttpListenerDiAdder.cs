using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.HttpListener.Interfaces;
using StreamProcessing.HttpListener.Logic;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.HttpListener;

internal sealed  class HttpListenerDiAdder : IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainIntroducer, HttpListenerGrainIntroducer>();
        collection.AddSingleton<IHttpListenerService, HttpListenerService>();
    }
}