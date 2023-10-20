using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Rest.Interfaces;

namespace StreamProcessing.Rest;

internal sealed class RestGrainIntroducer : IPluginGrainIntroducer
{
    public PluginTypeId PluginTypeId => new(PluginTypeNames.Rest);
    public Type GrainInterface => typeof(IRestGrain);
}