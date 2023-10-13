using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.PluginCommon.Interfaces;

internal interface IPluginOutputCaller
{
    Task CallOutputs(PluginExecutionContext pluginContext, List<PluginRecord> records, GrainCancellationToken cancellationToken);
}