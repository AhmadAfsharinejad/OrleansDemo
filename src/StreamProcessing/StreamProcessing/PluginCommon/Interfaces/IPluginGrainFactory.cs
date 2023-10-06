using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.PluginCommon.Interfaces;

internal interface IPluginGrainFactory
{
    IPluginGrain GetOrCreate(IGrainFactory grainFactory, PluginTypeId pluginTypeId, Guid pluginId);
}