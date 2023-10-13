using System.Data.Odbc;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.SqlExecutor.Domain;

namespace StreamProcessing.SqlExecutor.Interfaces;

internal interface ISqlExecutorService
{
    IAsyncEnumerable<PluginRecord> Execute(OdbcConnection connection,
        SqlExecutorConfig config,
        PluginRecord? record,
        CancellationToken cancellationToken);
}