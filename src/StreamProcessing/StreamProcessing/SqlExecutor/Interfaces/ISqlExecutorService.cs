using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.SqlExecutor.Domain;

namespace StreamProcessing.SqlExecutor.Interfaces;

internal interface ISqlExecutorService
{
    IAsyncEnumerable<PluginRecord> Execute(IStreamDbConnection connection,
        IStreamDbCommand command,
        SqlExecutorConfig config,
        PluginRecord? record,
        CancellationToken cancellationToken);
}