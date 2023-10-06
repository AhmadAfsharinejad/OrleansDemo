using Orleans.Concurrency;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.Scenario.Domain;

namespace StreamProcessing.Scenario.Interfaces;

internal interface IScenarioGrain : IGrainWithGuidKey
{
    Task AddScenario(ScenarioConfig config);
    [ReadOnly]
    Task<IPluginConfig> GetPluginConfig(Guid plugingId);
    [ReadOnly]
    Task<IReadOnlyCollection<PluginTypeWithId>> GetOutputTypes(Guid plugingId);
}