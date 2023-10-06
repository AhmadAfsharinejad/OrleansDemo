using Orleans.Concurrency;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.RandomGenerator.Domain;
using StreamProcessing.RandomGenerator.Interfaces;

namespace StreamProcessing.RandomGenerator;

internal sealed class RandomGeneratorGrain : PluginGrain<RandomGeneratorConfig>, IRandomGeneratorGrain
{
    [ReadOnly]
    [OneWay]
    public async Task Compute(Immutable<Guid> pluginId, Immutable<PluginRecords>? pluginRecords, GrainCancellationToken cancellationToken)
    {
      
        throw new NotImplementedException();
    }
}