using StreamProcessing.Filter.Interfaces;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.Filter;

internal sealed class FilterGrainIntroducer : IPluginGrainIntroducer
{
    public PluginTypeId PluginTypeId => new(PluginTypeNames.Filter);
    public Type GrainInterface => typeof(IFilterGrain);
}