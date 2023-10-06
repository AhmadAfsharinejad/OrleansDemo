using StreamProcessing.Aggregation.Interfaces;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.Aggregation;

internal sealed class AggregatorGrainIntroducer : IPluginGrainIntroducer
{
    public PluginTypeId PluginTypeId => new(PluginTypeNames.Aggregator.ToString());
    public Type GrainInterface => typeof(IAggregatorGrain);
}