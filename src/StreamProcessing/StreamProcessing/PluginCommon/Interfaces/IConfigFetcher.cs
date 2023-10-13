namespace StreamProcessing.PluginCommon.Interfaces;

internal interface IConfigFetcher<TConfig>
{
    Task<TConfig> GetConfig(Guid scenarioId, Guid pluginId);
}