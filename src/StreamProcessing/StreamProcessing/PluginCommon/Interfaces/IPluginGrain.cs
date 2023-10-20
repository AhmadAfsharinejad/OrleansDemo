using Orleans.Concurrency;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.PluginCommon.Interfaces;

internal interface IPluginGrain : IGrainWithGuidKey
{
    [ReadOnly]
    //[OneWay] --> Note: do not wait task
    Task Compute([Immutable] PluginExecutionContext pluginContext, 
        [Immutable] PluginRecords pluginRecords,
        GrainCancellationToken cancellationToken);
    
    [ReadOnly]
    Task Compute([Immutable] PluginExecutionContext pluginContext, 
        [Immutable] PluginRecord pluginRecord,
        GrainCancellationToken cancellationToken);
}