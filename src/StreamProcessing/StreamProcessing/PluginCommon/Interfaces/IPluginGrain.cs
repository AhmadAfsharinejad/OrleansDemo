using Orleans.Concurrency;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.PluginCommon.Interfaces;

internal interface IPluginGrain : IGrainWithGuidKey
{
    [ReadOnly]
    //[OneWay] --> Note: dge await nemishe
    Task Compute(Immutable<PluginExecutionContext> pluginContext, 
        Immutable<PluginRecords>? pluginRecords,
        GrainCancellationToken cancellationToken);
}