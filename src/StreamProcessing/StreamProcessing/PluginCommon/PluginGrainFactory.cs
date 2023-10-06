using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.PluginCommon;

internal sealed class PluginGrainFactory : IPluginGrainFactory
{
    private readonly IGrainFactory _grainFactory;
    private readonly Dictionary<PluginTypeId, Type> _pluginGrainTypeById;

    public PluginGrainFactory(IGrainFactory grainFactory, IEnumerable<IPluginGrainIntroducer> pluginGrainIntroducers)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        ArgumentNullException.ThrowIfNull(pluginGrainIntroducers);
        _pluginGrainTypeById = pluginGrainIntroducers.ToDictionary(x => x.PluginTypeId, y => y.GrainInterface);
    }

    public IPluginGrain GetOrCreate(PluginTypeId pluginTypeId, Guid pluginId)
    {
        if (!_pluginGrainTypeById.TryGetValue(pluginTypeId, out var grainType))
        {
            throw new KeyNotFoundException($"Plugin with type id '{pluginTypeId}' not found.");
        }

        if (_grainFactory.GetGrain(grainType, pluginId) is not IPluginGrain grain)
        {
            throw new InvalidCastException($"Plugin with type id '{pluginTypeId}' is not a 'IPluginGrain'.");
        }
        
        return grain;
    }
}