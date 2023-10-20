using Orleans.Concurrency;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.PluginCommon.Interfaces;

internal interface ISourcePluginGrain : IGrainWithGuidKey
{
    [ReadOnly]
    Task Start([Immutable] PluginExecutionContext pluginContext,
        GrainCancellationToken cancellationToken);
}