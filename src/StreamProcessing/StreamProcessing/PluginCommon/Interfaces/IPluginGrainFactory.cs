using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.PluginCommon.Interfaces;

internal interface IPluginGrainFactory
{
    IPluginGrain GetOrCreatePlugin(PluginTypeId pluginTypeId, Guid grainId);
    ISourcePluginGrain GetOrCreateSourcePlugin(PluginTypeId pluginTypeId, Guid grainId);
    ISourcePluginGrain GetOrCreateSourcePlugin(Type pluginType, Guid grainId);
}