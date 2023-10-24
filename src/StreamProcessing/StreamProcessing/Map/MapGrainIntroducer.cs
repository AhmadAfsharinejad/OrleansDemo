using StreamProcessing.Map.Interfaces;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.Map;

public class MapGrainIntroducer : IPluginGrainIntroducer
{
    public PluginTypeId PluginTypeId => new(PluginTypeNames.Map);
    public Type GrainInterface => typeof(IMapGrain);
}