using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Silo.Interfaces;

//Run source plugin in all silo
internal interface IEachSiloCaller
{
    Task Start([Immutable] Type startingPluginType, 
        [Immutable] PluginExecutionContext pluginContext,
        GrainCancellationToken cancellationToken);
}