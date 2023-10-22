using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Silo.Interfaces;

/// <summary>
/// local grain per silo which using for create source plugins
/// </summary>
internal interface ILocalSiloGrain : IGrainWithGuidKey
{
    Task SubscribeToCoordinator();
    Task UnSubscribeToCoordinator();

    Task StartPlugin([Immutable] Type startingPluginType,
        [Immutable] PluginExecutionContext pluginContext,
        GrainCancellationToken cancellationToken);
}