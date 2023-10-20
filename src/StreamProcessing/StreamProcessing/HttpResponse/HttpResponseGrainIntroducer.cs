using StreamProcessing.HttpResponse.Interfaces;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.HttpResponse;

internal sealed class HttpResponseGrainIntroducer : IPluginGrainIntroducer
{
    public PluginTypeId PluginTypeId => new(PluginTypeNames.HttpResponse);
    public Type GrainInterface => typeof(IHttpResponseGrain);
}