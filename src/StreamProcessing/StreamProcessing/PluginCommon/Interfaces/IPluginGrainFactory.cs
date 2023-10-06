using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.PluginCommon.Interfaces;

internal interface IPluginGrainFactory
{
    IPluginGrain GetOrCreate(PluginTypeId pluginTypeId, Guid pluginId);
}