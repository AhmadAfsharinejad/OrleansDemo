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

    public IPluginGrain GetOrCreatePlugin(PluginTypeId pluginTypeId, Guid grainId)
    {
        if (GetGrain(pluginTypeId, grainId) is not IPluginGrain grain)
        {
            throw new InvalidCastException($"Plugin with type id '{pluginTypeId}' is not a 'IPluginGrain'.");
        }

        return grain;
    }
    
    public ISourcePluginGrain GetOrCreateSourcePlugin(PluginTypeId pluginTypeId, Guid grainId)
    {
        if (GetGrain(pluginTypeId, grainId) is not ISourcePluginGrain grain)
        {
            throw new InvalidCastException($"Plugin with type id '{pluginTypeId}' is not a 'ISourcePluginGrain'.");
        }
        
        return grain;
    }
    
    public ISourcePluginGrain GetOrCreateSourcePlugin(Type pluginType, Guid grainId)
    {
        var grain = _grainFactory.GetGrain(pluginType, grainId);
        if (grain is not ISourcePluginGrain sourceGrain)
        {
            throw new InvalidCastException($"Plugin with type '{pluginType}' is not a 'ISourcePluginGrain'.");
        }
        
        return sourceGrain;
    }
    
    private IGrain GetGrain(PluginTypeId pluginTypeId, Guid grainId)
    {
        if (!_pluginGrainTypeById.TryGetValue(pluginTypeId, out var grainType))
        {
            throw new KeyNotFoundException($"Plugin with type id '{pluginTypeId}' not found.");
        }

        return _grainFactory.GetGrain(grainType, grainId);
    }
}