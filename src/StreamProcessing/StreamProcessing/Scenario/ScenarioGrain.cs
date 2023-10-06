using Orleans.Concurrency;
using Orleans.Runtime;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.Scenario.Domain;
using StreamProcessing.Scenario.Interfaces;
using StreamProcessing.Silo;

namespace StreamProcessing.Scenario;

internal sealed class ScenarioGrain : Grain, IScenarioGrain
{
    private readonly IPersistentState<ScenarioConfig> _confgiState;

    public ScenarioGrain(
        [PersistentState(stateName: "scenarioConfigState", storageName: SiloConsts.StorageName)] IPersistentState<ScenarioConfig> configState)
    {
        _confgiState = configState;
    }
    
    public async Task AddScenario(ScenarioConfig config)
    {
        _confgiState.State = config; 
        await _confgiState.WriteStateAsync();
    }
    
    [ReadOnly]
    public Task<IPluginConfig> GetPluginConfig(Guid plugingId)
    {
        var config = _confgiState.State.Configs.FirstOrDefault(x => x.Id == plugingId);

        if (config.Equals(default(PluginConfig)))
        {
            throw new Exception($"Config for plugin '{plugingId}' not exist.");
        }
        
        return Task.FromResult(config.Config);
    }
    
    [ReadOnly]
    public Task<IReadOnlyCollection<PluginTypeWithId>> GetOutputTypes(Guid plugingId)
    {
        var outputIds = _confgiState.State.Relations
            .Where(x => x.SourceId == plugingId)
            .Select(x => x.DestinationId).ToHashSet();

        var outputs = _confgiState.State.Configs
            .Where(x => outputIds.Contains(x.Id))
            .Select(x => new PluginTypeWithId(x.Id, x.PluginTypeId))
            .ToArray();

        return Task.FromResult(outputs as IReadOnlyCollection<PluginTypeWithId>);
    }
}