using Orleans.Concurrency;
using Orleans.Runtime;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.Scenario.Domain;
using StreamProcessing.Scenario.Interfaces;
using StreamProcessing.Silo;

namespace StreamProcessing.Scenario;

internal sealed class ScenarioGrain : Grain, IScenarioGrain
{
    private readonly IPersistentState<ScenarioConfig> _configState;

    public ScenarioGrain(
        [PersistentState(stateName: "scenarioConfigState", storageName: SiloConsts.StorageName)] IPersistentState<ScenarioConfig> configState)
    {
        _configState = configState;
    }
    
    public async Task AddScenario(ScenarioConfig config)
    {
        _configState.State = config; 
        await _configState.WriteStateAsync();
    }
    
    [ReadOnly]
    public Task<IPluginConfig> GetPluginConfig(Guid pluginId)
    {
        var config = _configState.State.Configs.FirstOrDefault(x => x.Id == pluginId);

        if (config.Equals(default(PluginConfig)))
        {
            throw new Exception($"Config for plugin '{pluginId}' not exist.");
        }
        
        return Task.FromResult(config.Config);
    }
    
    [ReadOnly]
    public Task<IReadOnlyCollection<PluginTypeWithId>> GetOutputTypes(Guid pluginId)
    {
        var outputIds = _configState.State.Relations
            .Where(x => x.SourceId == pluginId)
            .Select(x => x.DestinationId).ToHashSet();

        var outputs = _configState.State.Configs
            .Where(x => outputIds.Contains(x.Id))
            .Select(x => new PluginTypeWithId(x.Id, x.PluginTypeId))
            .ToArray();

        return Task.FromResult(outputs as IReadOnlyCollection<PluginTypeWithId>);
    }
}