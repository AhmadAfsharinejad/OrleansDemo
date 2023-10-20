using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Silo.Interfaces;

internal interface IEachSiloCaller
{
    Task Start([Immutable] Type startingPluginType, 
        [Immutable] PluginExecutionContext pluginContext,
        GrainCancellationToken cancellationToken);
}