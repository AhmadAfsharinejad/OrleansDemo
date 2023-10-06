using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.PluginCommon;

internal sealed class PluginGrainFactory : IPluginGrainFactory
{
    private readonly Dictionary<PluginTypeId, Type> _pluginGrainTypeById;

    public PluginGrainFactory(IEnumerable<IPluginGrainIntroducer> pluginGrainIntroducers)
    {
        ArgumentNullException.ThrowIfNull(pluginGrainIntroducers);
        _pluginGrainTypeById = pluginGrainIntroducers.ToDictionary(x => x.PluginTypeId, y => y.GrainInterface);
    }

    public IPluginGrain GetOrCreate(IGrainFactory grainFactory, PluginTypeId pluginTypeId, Guid pluginId)
    {
        if (!_pluginGrainTypeById.TryGetValue(pluginTypeId, out var grainType))
        {
            throw new KeyNotFoundException($"Plugin with type id '{pluginTypeId}' not found.");
        }

        if (grainFactory.GetGrain(grainType, pluginId) is not IPluginGrain grain)
        {
            throw new InvalidCastException($"Plugin with type id '{pluginTypeId}' is not a 'IPluginGrain'.");
        }
        
        return grain;
    }
}