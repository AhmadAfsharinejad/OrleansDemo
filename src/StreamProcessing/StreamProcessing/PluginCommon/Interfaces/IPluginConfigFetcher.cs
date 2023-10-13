namespace StreamProcessing.PluginCommon.Interfaces;

internal interface IPluginConfigFetcher<TConfig>
{
    Task<TConfig> GetConfig(Guid scenarioId, Guid pluginId);
}