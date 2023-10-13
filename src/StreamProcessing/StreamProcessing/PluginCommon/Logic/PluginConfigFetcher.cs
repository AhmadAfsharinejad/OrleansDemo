using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Scenario.Interfaces;

namespace StreamProcessing.PluginCommon.Logic;

internal sealed class PluginConfigFetcher<TConfig> : IPluginConfigFetcher<TConfig>
{
    private TConfig? _config;
    private readonly IGrainFactory _grainFactory;

    public PluginConfigFetcher(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
    }

    public async Task<TConfig> GetConfig(Guid scenarioId, Guid pluginId)
    {
        if (!(_config is null || _config!.Equals(default(TConfig)))) return _config;

        var scenarioGrain = _grainFactory.GetGrain<IScenarioGrain>(scenarioId);
        var config = await scenarioGrain.GetPluginConfig(pluginId);

        if (config is not TConfig tConfig) throw new InvalidCastException($"Can't cast plugin '{pluginId}' to specific type.");

        _config = tConfig;
        return _config;
    }
}