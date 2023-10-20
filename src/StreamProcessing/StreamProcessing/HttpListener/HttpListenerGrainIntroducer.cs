using StreamProcessing.HttpListener.Interfaces;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.HttpListener;

internal sealed class HttpListenerGrainIntroducer : IPluginGrainIntroducer
{
    public PluginTypeId PluginTypeId => new(PluginTypeNames.HttpListener);
    public Type GrainInterface => typeof(IHttpListenerGrain);
}