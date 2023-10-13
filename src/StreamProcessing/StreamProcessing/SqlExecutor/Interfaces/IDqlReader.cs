using System.Data.Odbc;
using StreamProcessing.SqlExecutor.Domain;

namespace StreamProcessing.SqlExecutor.Interfaces;

internal interface IDqlReader
{
    IAsyncEnumerable<IReadOnlyDictionary<string, object>>  
        Read(OdbcConnection connection, DqlCommand dqlCommand, IReadOnlyDictionary<string, object>? record, CancellationToken cancellationToken);
}