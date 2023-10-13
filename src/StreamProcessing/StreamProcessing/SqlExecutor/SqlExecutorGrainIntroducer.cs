using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.SqlExecutor.Interfaces;

namespace StreamProcessing.SqlExecutor;

internal sealed class SqlExecutorGrainIntroducer : IPluginGrainIntroducer
{
    public PluginTypeId PluginTypeId => new(PluginTypeNames.SqlExecutor);
    public Type GrainInterface => typeof(ISqlExecutorGrain);
}